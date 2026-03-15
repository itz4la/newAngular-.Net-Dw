// =============================================================================
// FILE    : Unit/UserControllerTests.cs
// PURPOSE : Unit tests for UserController (register / login / CRUD)
// LEVEL   : Unit
// TYPE    : Functional – Black-box (equivalence classes)
//           Security – basic access-control & authentication validation
// NOTE    : Login/Register endpoints depend on ASP.NET Identity (UserManager)
//           which is not easily mockable via interfaces – those methods are
//           covered in integration tests. This file tests the repo-dependent
//           CRUD actions only.
// =============================================================================

using api.Controllers;
using api.DTOs.Book;
using api.DTOs.User;
using api.Repositories.User;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Moq;
using Xunit;

namespace api.Tests.Unit
{
    public class UserControllerTests
    {
        private readonly Mock<IUserRepository> _userRepoMock;

        // UserController requires UserManager, RoleManager, IConfiguration too
        // For the CRUD endpoints that only use _userRepository we pass nulls
        // (they are never invoked in these paths)
        private UserController BuildSut() =>
            new UserController(
                _userRepoMock.Object,
                userManager:    null!,
                roleManager:    null!,
                configuration:  null!);

        public UserControllerTests()
        {
            _userRepoMock = new Mock<IUserRepository>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-USER-001  GetAll – returns 200 with paged user list
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetAll_ReturnsOkWithPagedUsers()
        {
            var paged = new PagedResultDto<UserDto>
            {
                Items = new List<UserDto>
                {
                    new UserDto { Id = "1", UserName = "john.doe", Email = "john@lib.com", Role = "Client" }
                },
                TotalCount = 1, PageNumber = 1, PageSize = 10
            };
            _userRepoMock.Setup(r => r.GetAllAsync(It.IsAny<UserFilterDto>())).ReturnsAsync(paged);

            var sut    = BuildSut();
            var result = await sut.GetAll(new UserFilterDto());

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-USER-002  GetById – valid ID → 200 OK
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var user = new UserDto { Id = "u1", UserName = "jane", Email = "jane@lib.com", Role = "Client" };
            _userRepoMock.Setup(r => r.GetByIdAsync("u1")).ReturnsAsync(user);

            var sut    = BuildSut();
            var result = await sut.GetById("u1");

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-USER-003  GetById – non-existent ID → 404 NotFound
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_NonExistent_ReturnsNotFound()
        {
            _userRepoMock.Setup(r => r.GetByIdAsync("ghost")).ReturnsAsync((UserDto?)null);

            var sut    = BuildSut();
            var result = await sut.GetById("ghost");

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-USER-004  CreateAdmin – valid DTO → 201 Created
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task CreateAdmin_ValidDto_ReturnsCreated()
        {
            var dto  = new CreateAdminUserDto { UserName = "admin2", Email = "admin2@lib.com", Password = "Admin@123" };
            var user = new UserDto { Id = "a2", UserName = "admin2", Email = "admin2@lib.com", Role = "Admin" };
            _userRepoMock.Setup(r => r.CreateAdminAsync(dto)).ReturnsAsync((true, null, user));

            var sut    = BuildSut();
            var result = await sut.CreateAdmin(dto);

            result.Result.Should().BeOfType<CreatedAtActionResult>()
                  .Which.StatusCode.Should().Be(201);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-USER-005  CreateAdmin – duplicate username → 400 BadRequest
        // Technique : Equivalence class (invalid: duplicate)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task CreateAdmin_DuplicateUsername_ReturnsBadRequest()
        {
            var dto = new CreateAdminUserDto { UserName = "admin", Email = "a@b.com", Password = "Admin@123" };
            _userRepoMock.Setup(r => r.CreateAdminAsync(dto)).ReturnsAsync((false, "Username already exists", null));

            var sut    = BuildSut();
            var result = await sut.CreateAdmin(dto);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-USER-006  Update – valid ID + valid DTO → 200 OK
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Update_ValidIdAndDto_ReturnsOk()
        {
            var dto  = new UpdateUserDto { UserName = "john.updated", Email = "john.updated@lib.com" };
            var user = new UserDto { Id = "u1", UserName = "john.updated", Email = "john.updated@lib.com", Role = "Client" };
            _userRepoMock.Setup(r => r.UpdateAsync("u1", dto)).ReturnsAsync((true, null, user));

            var sut    = BuildSut();
            var result = await sut.Update("u1", dto);

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-USER-007  Update – non-existent user → 404 NotFound
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Update_NonExistentUser_ReturnsNotFound()
        {
            var dto = new UpdateUserDto { UserName = "ghost", Email = "ghost@lib.com" };
            _userRepoMock.Setup(r => r.UpdateAsync("nobody", dto)).ReturnsAsync((false, "User not found", null));

            var sut    = BuildSut();
            var result = await sut.Update("nobody", dto);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-USER-008  Delete – valid ID → 204 NoContent
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Delete_ValidId_ReturnsNoContent()
        {
            _userRepoMock.Setup(r => r.DeleteAsync("u1")).ReturnsAsync((true, null));

            var sut    = BuildSut();
            var result = await sut.Delete("u1");

            result.Should().BeOfType<NoContentResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-USER-009  Delete – non-existent user → 404 NotFound
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Delete_NonExistentUser_ReturnsNotFound()
        {
            _userRepoMock.Setup(r => r.DeleteAsync("nobody")).ReturnsAsync((false, "User not found"));

            var sut    = BuildSut();
            var result = await sut.Delete("nobody");

            result.Should().BeOfType<NotFoundObjectResult>();
        }
    }
}
