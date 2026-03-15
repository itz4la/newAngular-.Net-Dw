// =============================================================================
// FILE    : Unit/GenreControllerTests.cs
// PURPOSE : Unit tests for GenreController (CRUD operations)
// LEVEL   : Unit
// TYPE    : Functional – Black-box (equivalence classes + boundary values)
// =============================================================================

using api.Controllers;
using api.DTOs.Book;
using api.DTOs.Genre;
using api.Repositories.Genre;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace api.Tests.Unit
{
    public class GenreControllerTests
    {
        private readonly Mock<IGenreRepository> _repoMock;
        private readonly GenreController        _sut;

        public GenreControllerTests()
        {
            _repoMock = new Mock<IGenreRepository>();
            _sut      = new GenreController(_repoMock.Object);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-GENRE-001  GetAll – returns 200 with full genre list
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetAll_ReturnsOkWithGenreList()
        {
            // Arrange
            var genres = new List<GenreDto>
            {
                new GenreDto { Id = 1, Name = "Fiction"          },
                new GenreDto { Id = 2, Name = "Science Fiction"  },
                new GenreDto { Id = 3, Name = "Mystery"          },
                new GenreDto { Id = 4, Name = "Biography"        }
            };
            _repoMock.Setup(r => r.GetAllAsync()).ReturnsAsync(genres);

            // Act
            var result = await _sut.GetAll();

            // Assert
            var ok   = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var list = ok.Value.Should().BeAssignableTo<List<GenreDto>>().Subject;
            list.Should().HaveCount(4);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-GENRE-002  GetById – valid ID → 200 OK with correct GenreDto
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_ValidId_ReturnsOkWithGenre()
        {
            // Arrange
            var genre = new GenreDto { Id = 1, Name = "Fiction" };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(genre);

            // Act
            var result = await _sut.GetById(1);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(genre);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-GENRE-003  GetById – ID ≤ 0 → 400 BadRequest
        // Technique : Boundary value analysis
        // ─────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task GetById_InvalidId_ReturnsBadRequest(int badId)
        {
            var result = await _sut.GetById(badId);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-GENRE-004  GetById – non-existent ID → 404 NotFound
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_NonExistent_ReturnsNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(42)).ReturnsAsync((GenreDto?)null);
            var result = await _sut.GetById(42);
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-GENRE-005  Create – unique name → 201 Created
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Create_UniqueName_ReturnsCreated()
        {
            // Arrange
            var dto     = new CreateGenreDto { Name = "Horror" };
            var created = new GenreDto { Id = 5, Name = "Horror" };
            _repoMock.Setup(r => r.CreateAsync(dto)).ReturnsAsync(created);

            // Act
            var result = await _sut.Create(dto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-GENRE-006  Create – duplicate name → 400 BadRequest
        // Technique : Equivalence class (duplicate partition)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Create_DuplicateName_ReturnsBadRequest()
        {
            // Arrange
            var dto = new CreateGenreDto { Name = "Fiction" };
            _repoMock.Setup(r => r.CreateAsync(dto)).ReturnsAsync((GenreDto?)null);

            // Act
            var result = await _sut.Create(dto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-GENRE-007  Update – valid ID + valid DTO → 200 OK
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Update_ValidIdAndDto_ReturnsOk()
        {
            var dto     = new UpdateGenreDto { Name = "Thriller" };
            var updated = new GenreDto { Id = 1, Name = "Thriller" };
            _repoMock.Setup(r => r.UpdateAsync(1, dto)).ReturnsAsync(updated);

            var result = await _sut.Update(1, dto);

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-GENRE-008  Update – non-existent genre → 404 NotFound
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Update_NonExistentGenre_ReturnsNotFound()
        {
            var dto = new UpdateGenreDto { Name = "Ghost Genre" };
            _repoMock.Setup(r => r.UpdateAsync(99, dto)).ReturnsAsync((GenreDto?)null);
            _repoMock.Setup(r => r.GenreExistsAsync(99)).ReturnsAsync(false);

            var result = await _sut.Update(99, dto);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-GENRE-009  GetAllWithPagination – returns paged result
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetAllWithPagination_ReturnsOkWithPagedResult()
        {
            var paged = new PagedResultDto<GenreDto>
            {
                Items      = new List<GenreDto> { new GenreDto { Id = 1, Name = "Fiction" } },
                TotalCount = 4,
                PageNumber = 1,
                PageSize   = 1
            };
            var filter = new GenreFilterDto { PageNumber = 1, PageSize = 1 };
            _repoMock.Setup(r => r.GetAllWithPaginationAsync(It.IsAny<GenreFilterDto>())).ReturnsAsync(paged);

            var result = await _sut.GetAllWithPagination(filter);

            result.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}
