// =============================================================================
// FILE    : Integration/UserApiIntegrationTests.cs
// PURPOSE : HTTP-level integration tests for /api/user (register / login / CRUD)
// LEVEL   : Integration
// TYPE    : Functional + Security (authentication boundary)
// =============================================================================

using System.Net;
using System.Net.Http.Json;
using FluentAssertions;
using Xunit;

namespace api.Tests.Integration
{
    public class UserApiIntegrationTests : IClassFixture<LibraryWebAppFactory>
    {
        private readonly HttpClient _client;

        public UserApiIntegrationTests(LibraryWebAppFactory factory)
        {
            _client = factory.CreateClient();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-USER-001  POST /api/user/register – valid payload → 200 OK
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Register_ValidUser_ReturnsOk()
        {
            var dto = new
            {
                Username = $"testuser_{Guid.NewGuid():N}",
                Email    = $"test_{Guid.NewGuid():N}@library.com",
                Password = "Test@1234!"
            };

            var response = await _client.PostAsJsonAsync("/api/user/register", dto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-USER-002  POST /api/user/register – duplicate username → 409
        // Technique : Equivalence class (conflict partition)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Register_DuplicateUsername_ReturnsConflict()
        {
            var dto = new
            {
                Username = "duplicate_conflict_user",
                Email    = "dup@library.com",
                Password = "Test@1234!"
            };

            // First registration should succeed
            await _client.PostAsJsonAsync("/api/user/register", dto);

            // Second registration with same username → Conflict
            var response = await _client.PostAsJsonAsync("/api/user/register", dto);

            response.StatusCode.Should().Be(HttpStatusCode.Conflict);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-USER-003  POST /api/user/login – valid credentials → 200 + JWT
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Login_ValidCredentials_ReturnsTokenAndUsername()
        {
            // First register the user
            var username = $"loginuser_{Guid.NewGuid():N}";
            await _client.PostAsJsonAsync("/api/user/register", new
            {
                Username = username,
                Email    = $"{username}@library.com",
                Password = "Test@1234!"
            });

            // Now login
            var loginDto = new { Username = username, Password = "Test@1234!" };
            var response = await _client.PostAsJsonAsync("/api/user/login", loginDto);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var body = await response.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            body.Should().ContainKey("token");
            body.Should().ContainKey("username");
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-USER-004  POST /api/user/login – wrong password → 400
        // Technique : Security – invalid credential boundary
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Login_WrongPassword_ReturnsBadRequest()
        {
            // Register first
            var username = $"wrongpass_{Guid.NewGuid():N}";
            await _client.PostAsJsonAsync("/api/user/register", new
            {
                Username = username,
                Email    = $"{username}@library.com",
                Password = "Correct@1234!"
            });

            // Attempt login with wrong password
            var response = await _client.PostAsJsonAsync("/api/user/login", new
            {
                Username = username,
                Password = "WrongPassword@999"
            });

            response.StatusCode.Should().Be(HttpStatusCode.BadRequest);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-USER-005  POST /api/user/login – non-existent username → 401
        // Technique : Security – unknown user
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Login_NonExistentUser_ReturnsUnauthorized()
        {
            var response = await _client.PostAsJsonAsync("/api/user/login", new
            {
                Username = "this_user_does_not_exist_xyz",
                Password = "SomePassword@1"
            });

            response.StatusCode.Should().Be(HttpStatusCode.Unauthorized);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-USER-006  GET /api/user – returns 200 with paged list
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetAllUsers_ReturnsOk()
        {
            var response = await _client.GetAsync("/api/user");
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-I-USER-007  GET /api/user/{id} – non-existent → 404
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetUserById_NonExistent_Returns404()
        {
            var response = await _client.GetAsync("/api/user/non-existent-id-xyz");
            response.StatusCode.Should().Be(HttpStatusCode.NotFound);
        }
    }
}
