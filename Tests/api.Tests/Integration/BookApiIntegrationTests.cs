// =============================================================================
// FILE    : Integration/BookApiIntegrationTests.cs
// PURPOSE : HTTP-level integration tests for the /api/book endpoint.
//           Uses LibraryWebAppFactory with in-memory DB (no real SQL Server).
// LEVEL   : Integration
// TYPE    : Functional (API contract) + Regression
// =============================================================================

using System.Net;
using System.Net.Http.Json;
using api.DTOs.Book;
using FluentAssertions;
using Xunit;

namespace api.Tests.Integration
{
    public class BookApiIntegrationTests : IClassFixture<LibraryWebAppFactory>
    {
        private readonly HttpClient _client;

        public BookApiIntegrationTests(LibraryWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-BOOK-001  GET /api/book – returns 200 with paged result
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetAll_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/book");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-BOOK-002  GET /api/book/{id} – non-existent ID → 404
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_NonExistentId_Returns404()
        {
            var response = await _client.GetAsync("/api/book/99999");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-BOOK-003  GET /api/book/{id} – invalid (0) ID → 400
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_ZeroId_Returns400()
        {
            var response = await _client.GetAsync("/api/book/0");
            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-BOOK-004  POST /api/book – invalid payload (missing title) → 400
        // Technique : Black-box – required field missing
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Create_MissingTitle_Returns400()
        {
            // Missing required Title field
            var dto = new
            {
                Author        = "Test Author",
                GenreId       = 1,
                PublishedDate = "2020-01-01",
                CoverImageUrl = "https://example.com/img.jpg"
            };

            var response = await _client.PostAsJsonAsync("/api/book", dto);

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-BOOK-005  GET /api/book/available – returns 200
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetAvailable_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/book/available");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-BOOK-006  DELETE /api/book/{id} – non-existent → 404
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Delete_NonExistentId_Returns404()
        {
            var response = await _client.DeleteAsync("/api/book/88888");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-BOOK-007  Full CRUD round-trip: Create → GetById → Delete
        // Technique : Integration – module interaction (Genre + Book dependencies)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task FullCrudRoundTrip_CreateGetDelete_Succeeds()
        {
            // 1. Create genre first (Book depends on Genre)
            var genreDto = new { Name = "Integration Test Genre" };
            var genreResponse = await _client.PostAsJsonAsync("/api/genre", genreDto);
            genreResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            var genre = await genreResponse.Content.ReadFromJsonAsync<api.DTOs.Genre.GenreDto>();

            // 2. Create book using the genre
            var bookDto = new
            {
                Title         = "Integration Test Book",
                Author        = "Test Author",
                GenreId       = genre!.Id,
                PublishedDate = "2023-06-15T00:00:00",
                CoverImageUrl = "https://test.com/book.jpg",
                Description   = "A test book for integration testing"
            };
            var createResponse = await _client.PostAsJsonAsync("/api/book", bookDto);
            createResponse.StatusCode.Should().Be(HttpStatusCode.Created);

            var created = await createResponse.Content.ReadFromJsonAsync<BookDto>();
            created.Should().NotBeNull();
            created!.Title.Should().Be("Integration Test Book");

            // 3. GetById
            var getResponse = await _client.GetAsync($"/api/book/{created.Id}");
            getResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            // 4. Delete
            var deleteResponse = await _client.DeleteAsync($"/api/book/{created.Id}");
            deleteResponse.StatusCode.Should().Be(HttpStatusCode.NoContent);

            // 5. Verify deletion
            var afterDelete = await _client.GetAsync($"/api/book/{created.Id}");
            afterDelete.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
