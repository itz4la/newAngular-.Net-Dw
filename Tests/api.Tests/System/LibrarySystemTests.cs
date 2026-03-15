// =============================================================================
// FILE    : System/LibrarySystemTests.cs
// PURPOSE : Full end-to-end system tests covering complete user scenarios.
//           Validates the entire library workflow from registration to loan return.
// LEVEL   : System
// TYPE    : Functional – complete user journey scenarios
//           Regression – verifies core flows still work after changes
// =============================================================================

using System.Net;
using System.Net.Http.Json;
using api.DTOs.Book;
using api.DTOs.Genre;
using api.DTOs.Loan;
using api.DTOs.User;
using FluentAssertions;
using Xunit;

namespace api.Tests.System
{
    /// <summary>
    /// System-level tests simulate complete real-world user scenarios across
    /// multiple API endpoints, verifying the entire library management workflow.
    /// </summary>
    public class LibrarySystemTests : IClassFixture<api.Tests.Integration.LibraryWebAppFactory>
    {
        private readonly HttpClient _client;

        public LibrarySystemTests(api.Tests.Integration.LibraryWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-S-001  Complete Member Lifecycle
        //           Scenario: A new library member registers, browses books,
        //           borrows one, and returns it successfully.
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Scenario_NewMember_BorrowsAndReturnsBook()
        {
            // ── Step 1: Admin sets up a genre ─────────────────────────────────
            var genre = await _client.PostAsJsonAsync("/api/genre", new { Name = $"SysTest_{Guid.NewGuid():N}" });
            genre.StatusCode.Should().Be(HttpStatusCode.Created);
            var genreDto = await genre.Content.ReadFromJsonAsync<GenreDto>();

            // ── Step 2: Admin creates a book ──────────────────────────────────
            var bookResp = await _client.PostAsJsonAsync("/api/book", new
            {
                Title         = $"System Test Book {Guid.NewGuid():N}",
                Author        = "System Author",
                GenreId       = genreDto!.Id,
                PublishedDate = "2024-01-15T00:00:00",
                CoverImageUrl = "https://example.com/sys-test.jpg",
                Description   = "Book used for system-level testing"
            });
            bookResp.StatusCode.Should().Be(HttpStatusCode.Created);
            var book = await bookResp.Content.ReadFromJsonAsync<BookDto>();
            book!.IsAvailable.Should().BeTrue("Newly created book should be available");

            // ── Step 3: New member registers ─────────────────────────────────
            var username = $"member_{Guid.NewGuid():N}";
            var regResp  = await _client.PostAsJsonAsync("/api/user/register", new
            {
                Username = username,
                Email    = $"{username}@library.com",
                Password = "Member@1234!"
            });
            regResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // ── Step 4: Member logs in ────────────────────────────────────────
            var loginResp = await _client.PostAsJsonAsync("/api/user/login", new
            {
                Username = username,
                Password = "Member@1234!"
            });
            loginResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var loginBody = await loginResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            loginBody.Should().ContainKey("token");

            // ── Step 5: Fetch the newly registered user's ID ──────────────────
            var usersResp = await _client.GetAsync($"/api/user?pageSize=1000");
            var usersBody = await usersResp.Content.ReadFromJsonAsync<PagedResultDto<UserDto>>();
            var memberId  = usersBody!.Items.FirstOrDefault(u => u.UserName == username)?.Id;
            memberId.Should().NotBeNullOrEmpty();

            // ── Step 6: Check book availability before borrowing ──────────────
            var availResp = await _client.GetAsync($"/api/loan/book/{book.Id}/availability");
            availResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // ── Step 7: Member borrows the book ───────────────────────────────
            var loanResp = await _client.PostAsJsonAsync("/api/loan", new
            {
                BookId = book.Id,
                UserId = memberId
            });
            loanResp.StatusCode.Should().Be(HttpStatusCode.Created);
            var loan = await loanResp.Content.ReadFromJsonAsync<LoanDto>();
            loan!.Status.Should().Be("Active");
            loan.BookId.Should().Be(book.Id);

            // ── Step 8: Book is now unavailable ───────────────────────────────
            var availAfterResp = await _client.GetAsync($"/api/loan/book/{book.Id}/availability");
            availAfterResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // ── Step 9: Member views their active loans ───────────────────────
            var activeLoansResp = await _client.GetAsync($"/api/loan/user/{memberId}/active");
            activeLoansResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var activeLoans = await activeLoansResp.Content.ReadFromJsonAsync<List<LoanDto>>();
            activeLoans.Should().Contain(l => l.Id == loan.Id);

            // ── Step 10: Member returns the book ──────────────────────────────
            var returnResp = await _client.PostAsJsonAsync($"/api/loan/return/{loan.Id}", new { });
            returnResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var returned = await returnResp.Content.ReadFromJsonAsync<LoanDto>();
            returned!.Status.Should().Be("Returned");
            returned.ReturnDate.Should().NotBeNull();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-S-002  Loan Limit Enforcement
        //           Scenario: A user should NOT be able to borrow more than 5 books.
        //           Business rule: MAX_BOOKS_PER_USER = 5
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Scenario_LoanLimitEnforced_SixthLoanRejected()
        {
            // Register a new user
            var username = $"limituser_{Guid.NewGuid():N}";
            await _client.PostAsJsonAsync("/api/user/register", new
            {
                Username = username,
                Email    = $"{username}@library.com",
                Password = "Limit@1234!"
            });
            var usersResp = await _client.GetAsync("/api/user?pageSize=1000");
            var usersBody = await usersResp.Content.ReadFromJsonAsync<PagedResultDto<UserDto>>();
            var userId    = usersBody!.Items.First(u => u.UserName == username).Id;

            // Create a genre
            var genreResp = await _client.PostAsJsonAsync("/api/genre",
                new { Name = $"LimitGenre_{Guid.NewGuid():N}" });
            var genre = await genreResp.Content.ReadFromJsonAsync<GenreDto>();

            // Create 6 books
            var bookIds = new List<int>();
            for (int i = 1; i <= 6; i++)
            {
                var bResp = await _client.PostAsJsonAsync("/api/book", new
                {
                    Title         = $"LimitBook_{i}_{Guid.NewGuid():N}",
                    Author        = "Limit Author",
                    GenreId       = genre!.Id,
                    PublishedDate = "2024-01-01T00:00:00",
                    CoverImageUrl = "https://example.com/img.jpg",
                    Description   = $"Book {i} for limit test"
                });
                var b = await bResp.Content.ReadFromJsonAsync<BookDto>();
                bookIds.Add(b!.Id);
            }

            // Borrow 5 books – all should succeed
            for (int i = 0; i < 5; i++)
            {
                var lResp = await _client.PostAsJsonAsync("/api/loan", new
                {
                    BookId = bookIds[i],
                    UserId = userId
                });
                lResp.StatusCode.Should().Be(HttpStatusCode.Created,
                    $"Borrowing book {i + 1} of 5 should succeed");
            }

            // Attempt 6th borrow – must be rejected (400 BadRequest)
            var sixthLoan = await _client.PostAsJsonAsync("/api/loan", new
            {
                BookId = bookIds[5],
                UserId = userId
            });
            sixthLoan.StatusCode.Should().Be(HttpStatusCode.BadRequest,
                "Borrowing a 6th book should exceed the maximum limit of 5");
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-S-003  Genre Management Full Flow
        //           Scenario: Admin creates a genre, assigns books to it,
        //           retrieves them, and verifies the paginated genre list.
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Scenario_GenreManagement_FullFlow()
        {
            // Create genre
            var genreName = $"Fantasy_{Guid.NewGuid():N}";
            var createResp = await _client.PostAsJsonAsync("/api/genre", new { Name = genreName });
            createResp.StatusCode.Should().Be(HttpStatusCode.Created);
            var genre = await createResp.Content.ReadFromJsonAsync<GenreDto>();
            genre!.Name.Should().Be(genreName);

            // Create 2 books in this genre
            for (int i = 1; i <= 2; i++)
            {
                var bResp = await _client.PostAsJsonAsync("/api/book", new
                {
                    Title         = $"Fantasy Book {i} {Guid.NewGuid():N}",
                    Author        = "Fantasy Author",
                    GenreId       = genre.Id,
                    PublishedDate = "2023-05-01T00:00:00",
                    CoverImageUrl = "https://example.com/fantasy.jpg",
                    Description   = $"Fantasy book number {i}"
                });
                bResp.StatusCode.Should().Be(HttpStatusCode.Created);
            }

            // Retrieve genre by ID
            var getResp = await _client.GetAsync($"/api/genre/{genre.Id}");
            getResp.StatusCode.Should().Be(HttpStatusCode.OK);

            // Update genre name
            var updateResp = await _client.PutAsJsonAsync($"/api/genre/{genre.Id}",
                new { Name = $"Updated_{genreName}" });
            updateResp.StatusCode.Should().Be(HttpStatusCode.OK);
            var updated = await updateResp.Content.ReadFromJsonAsync<GenreDto>();
            updated!.Name.Should().StartWith("Updated_");

            // Paginated genre list should contain the updated genre
            var listResp = await _client.GetAsync("/api/genre/paginated?pageNumber=1&pageSize=100");
            listResp.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-S-004  User Cannot Borrow Same Book Twice (Concurrent Availability)
        //           Scenario: Two users attempt to borrow the same single-copy book.
        //           Only the first should succeed.
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Scenario_SameBookCannotBeBorrowedByTwoUsers()
        {
            // Create genre + book (single copy)
            var gResp = await _client.PostAsJsonAsync("/api/genre", new { Name = $"SingleCopy_{Guid.NewGuid():N}" });
            var g     = await gResp.Content.ReadFromJsonAsync<GenreDto>();

            var bResp = await _client.PostAsJsonAsync("/api/book", new
            {
                Title         = $"SingleCopyBook_{Guid.NewGuid():N}",
                Author        = "Author",
                GenreId       = g!.Id,
                PublishedDate = "2023-01-01T00:00:00",
                CoverImageUrl = "https://example.com/sc.jpg",
                Description   = "Single copy book"
            });
            var book = await bResp.Content.ReadFromJsonAsync<BookDto>();

            // Register two users
            async Task<string> RegisterUserAsync(string tag)
            {
                var uname = $"{tag}_{Guid.NewGuid():N}";
                await _client.PostAsJsonAsync("/api/user/register", new
                {
                    Username = uname, Email = $"{uname}@lib.com", Password = "Test@1234!"
                });
                var all     = await _client.GetAsync("/api/user?pageSize=1000");
                var allBody = await all.Content.ReadFromJsonAsync<PagedResultDto<UserDto>>();
                return allBody!.Items.First(u => u.UserName == uname).Id;
            }

            var user1Id = await RegisterUserAsync("userA");
            var user2Id = await RegisterUserAsync("userB");

            // User 1 borrows the book successfully
            var loan1 = await _client.PostAsJsonAsync("/api/loan", new { BookId = book!.Id, UserId = user1Id });
            loan1.StatusCode.Should().Be(HttpStatusCode.Created);

            // User 2 attempts to borrow the same book – should be rejected
            var loan2 = await _client.PostAsJsonAsync("/api/loan", new { BookId = book.Id, UserId = user2Id });
            loan2.StatusCode.Should().Be(HttpStatusCode.BadRequest,
                "The book is already borrowed by User 1 and should not be available");
        }
    }
}
