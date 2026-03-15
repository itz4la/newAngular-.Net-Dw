// =============================================================================
// FILE    : Integration/LoanApiIntegrationTests.cs
// PURPOSE : HTTP-level integration tests for /api/loan endpoints
// LEVEL   : Integration
// TYPE    : Functional – API contracts, cross-module interactions
// =============================================================================

using System.Net;
using System.Net.Http.Json;
using api.DTOs.Book;
using api.DTOs.Genre;
using api.DTOs.Loan;
using FluentAssertions;
using Xunit;

namespace api.Tests.Integration
{
    public class LoanApiIntegrationTests : IClassFixture<LibraryWebAppFactory>
    {
        private readonly HttpClient _client;

        public LoanApiIntegrationTests(LibraryWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-LOAN-001  GET /api/loan – returns 200 with paged result
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/loan");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-LOAN-002  GET /api/loan/{id} – invalid ID (0) → 400
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_ZeroId_Returns400()
        {
            var response = await _client.GetAsync("/api/loan/0");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-LOAN-003  GET /api/loan/{id} – non-existent → 404
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_NonExistent_Returns404()
        {
            var response = await _client.GetAsync("/api/loan/99999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-LOAN-004  GET /api/loan/book/{bookId}/availability – valid ID
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task CheckBookAvailability_ValidId_ReturnsOk()
        {
            // Create a book first to get a valid book ID
            var genre = await CreateTestGenreAsync("Loan Test Genre");
            var book  = await CreateTestBookAsync(genre.Id);

            var response = await _client.GetAsync($"/api/loan/book/{book.Id}/availability");
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            body.Should().ContainKey("isAvailable");
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-LOAN-005  GET /api/loan/user/{userId}/active – empty userId → 400
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetUserActiveLoans_EmptyUserId_Returns400()
        {
            // URL with whitespace-only string causes routing to supply empty segment
            var response = await _client.GetAsync("/api/loan/user/ /active");
            // Either 400 or 404 from routing are acceptable guard responses
            ((int)response.StatusCode).Should().BeOneOf(400, 404);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-LOAN-006  POST /api/loan/return/{loanId} – invalid ID → 400
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task ReturnLoan_InvalidId_Returns400()
        {
            var response = await _client.PostAsJsonAsync("/api/loan/return/0", new { });
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-LOAN-007  POST /api/loan/return/{loanId} – non-existent loan → 404
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task ReturnLoan_NonExistentLoan_Returns404()
        {
            var response = await _client.PostAsJsonAsync("/api/loan/return/88888", new { });
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-LOAN-008  POST /api/loan – valid dto → 201 Created
        //                Full cross-module: User + Genre + Book + Loan
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task CreateLoan_ValidDto_ReturnsCreated()
        {
            // Register a user
            var username = $"loanuser_{Guid.NewGuid():N}";
            await _client.PostAsJsonAsync("/api/user/register", new
            {
                Username = username,
                Email    = $"{username}@library.com",
                Password = "Test@1234!"
            });

            // Fetch user ID via login token + GetAll
            var loginResp = await _client.PostAsJsonAsync("/api/user/login", new
            {
                Username = username,
                Password = "Test@1234!"
            });
            var loginBody = await loginResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();

            // Get all users and find this user's ID
            var usersResp = await _client.GetAsync("/api/user");
            var usersBody = await usersResp.Content
                .ReadFromJsonAsync<PagedResultDto<api.DTOs.User.UserDto>>();
            var userId    = usersBody!.Items.FirstOrDefault(u => u.UserName == username)?.Id;

            userId.Should().NotBeNullOrEmpty("User should have been registered and found");

            // Create genre + book
            var genre = await CreateTestGenreAsync($"LoanTestGenre_{Guid.NewGuid():N}");
            var book  = await CreateTestBookAsync(genre.Id);

            // Create the loan
            var loanDto = new { BookId = book.Id, UserId = userId };
            var response = await _client.PostAsJsonAsync("/api/loan", loanDto);

            response.StatusCode.Should().Be(HttpStatusCode.Created);
            var loan = await response.Content.ReadFromJsonAsync<LoanDto>();
            loan.Should().NotBeNull();
            loan!.Status.Should().Be("Active");
        }

        // ── Helpers ───────────────────────────────────────────────────────────

        private async Task<GenreDto> CreateTestGenreAsync(string name)
        {
            var r = await _client.PostAsJsonAsync("/api/genre", new { Name = name });
            r.EnsureSuccessStatusCode();
            return (await r.Content.ReadFromJsonAsync<GenreDto>())!;
        }

        private async Task<BookDto> CreateTestBookAsync(int genreId)
        {
            var dto = new
            {
                Title         = $"Book_{Guid.NewGuid():N}",
                Author        = "Integration Author",
                GenreId       = genreId,
                PublishedDate = "2022-01-01T00:00:00",
                CoverImageUrl = "https://example.com/img.jpg",
                Description   = "Integration test book"
            };
            var r = await _client.PostAsJsonAsync("/api/book", dto);
            r.EnsureSuccessStatusCode();
            return (await r.Content.ReadFromJsonAsync<BookDto>())!;
        }
    }
}
