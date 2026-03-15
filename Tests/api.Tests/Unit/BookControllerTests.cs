// =============================================================================
// FILE    : Unit/BookControllerTests.cs
// PURPOSE : Unit tests for BookController using Moq (no real DB, no real HTTP)
// LEVEL   : Unit
// TYPE    : Functional – Black-box (equivalence classes + boundary values)
// =============================================================================

using api.Controllers;
using api.DTOs.Book;
using api.Repositories.Book;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace api.Tests.Unit
{
    public class BookControllerTests
    {
        // ── Fixtures ──────────────────────────────────────────────────────────
        private readonly Mock<IBookRepository> _repoMock;
        private readonly BookController         _sut;   // System Under Test

        public BookControllerTests()
        {
            _repoMock = new Mock<IBookRepository>();
            _sut      = new BookController(_repoMock.Object);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-001  GetById – valid existing ID returns 200 with BookDto
        // Technique : Equivalence class (valid partition)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_ValidId_ReturnsOkWithBookDto()
        {
            // Arrange
            var expected = new BookDto
            {
                Id          = 1,
                Title       = "1984",
                Author      = "George Orwell",
                GenreId     = 2,
                GenreName   = "Science Fiction",
                IsAvailable = true
            };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(expected);

            // Act
            var result = await _sut.GetById(1);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(expected);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-002  GetById – ID = 0 (boundary value) returns 400
        // Technique : Boundary value analysis (lower bound violation)
        // ─────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-999)]
        public async Task GetById_InvalidId_ReturnsBadRequest(int invalidId)
        {
            // Act
            var result = await _sut.GetById(invalidId);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            _repoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-003  GetById – non-existent ID returns 404
        // Technique : Equivalence class (invalid partition – missing resource)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((BookDto?)null);

            // Act
            var result = await _sut.GetById(999);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-004  GetAll – returns 200 with paged result
        // Technique : Equivalence class (happy path)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetAll_ReturnsOkWithPagedResult()
        {
            // Arrange
            var paged = new PagedResultDto<BookDto>
            {
                Items      = new List<BookDto> { new BookDto { Id = 1, Title = "Dune" } },
                TotalCount = 1,
                PageNumber = 1,
                PageSize   = 10
            };
            var filter = new BookFilterDto { PageNumber = 1, PageSize = 10 };
            _repoMock.Setup(r => r.GetAllAsync(It.IsAny<BookFilterDto>())).ReturnsAsync(paged);

            // Act
            var result = await _sut.GetAll(filter);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(paged);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-005  Create – valid DTO → 201 Created with BookDto
        // Technique : Equivalence class (valid partition)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Create_ValidDto_ReturnsCreated()
        {
            // Arrange
            var createDto = new CreateBookDto
            {
                Title         = "The Hobbit",
                Author        = "J.R.R. Tolkien",
                GenreId       = 1,
                PublishedDate = new DateTime(1937, 9, 21),
                CoverImageUrl = "https://example.com/hobbit.jpg"
            };
            var created = new BookDto { Id = 5, Title = "The Hobbit", Author = "J.R.R. Tolkien" };
            _repoMock.Setup(r => r.CreateAsync(createDto)).ReturnsAsync(created);

            // Act
            var result = await _sut.Create(createDto);

            // Assert
            var createdResult = result.Result.Should().BeOfType<CreatedAtActionResult>().Subject;
            createdResult.StatusCode.Should().Be(201);
            createdResult.Value.Should().BeEquivalentTo(created);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-006  Create – invalid genre ID → 400 BadRequest
        // Technique : Equivalence class (invalid genre partition)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Create_InvalidGenre_ReturnsBadRequest()
        {
            // Arrange
            var createDto = new CreateBookDto
            {
                Title         = "Unknown Genre Book",
                Author        = "Author",
                GenreId       = 9999,
                PublishedDate = DateTime.Now
            };
            _repoMock.Setup(r => r.CreateAsync(createDto)).ReturnsAsync((BookDto?)null);

            // Act
            var result = await _sut.Create(createDto);

            // Assert
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-007  Update – valid ID + valid DTO → 200 OK
        // Technique : Equivalence class (valid partition)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Update_ValidIdAndDto_ReturnsOk()
        {
            // Arrange
            var updateDto = new UpdateBookDto
            {
                Title         = "1984 Updated",
                Author        = "George Orwell",
                GenreId       = 2,
                PublishedDate = new DateTime(1949, 6, 8)
            };
            var updated = new BookDto { Id = 1, Title = "1984 Updated" };
            _repoMock.Setup(r => r.UpdateAsync(1, updateDto)).ReturnsAsync(updated);

            // Act
            var result = await _sut.Update(1, updateDto);

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            ok.Value.Should().BeEquivalentTo(updated);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-008  Update – non-existent book → 404 NotFound
        // Technique : Equivalence class (missing resource)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Update_NonExistentBook_ReturnsNotFound()
        {
            // Arrange
            var updateDto = new UpdateBookDto
            {
                Title         = "Ghost",
                Author        = "Nobody",
                GenreId       = 1,
                PublishedDate = DateTime.Now
            };
            _repoMock.Setup(r => r.UpdateAsync(500, updateDto)).ReturnsAsync((BookDto?)null);
            _repoMock.Setup(r => r.BookExistsAsync(500)).ReturnsAsync(false);

            // Act
            var result = await _sut.Update(500, updateDto);

            // Assert
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-009  Delete – valid existing ID → 204 NoContent
        // Technique : Equivalence class (valid partition)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Delete_ValidId_ReturnsNoContent()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(1)).ReturnsAsync(true);

            // Act
            var result = await _sut.Delete(1);

            // Assert
            result.Should().BeOfType<NoContentResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-010  Delete – non-existent ID → 404 NotFound
        // Technique : Equivalence class (missing resource)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task Delete_NonExistentId_ReturnsNotFound()
        {
            // Arrange
            _repoMock.Setup(r => r.DeleteAsync(999)).ReturnsAsync(false);

            // Act
            var result = await _sut.Delete(999);

            // Assert
            result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-BOOK-011  GetAvailable – returns list of available books
        // Technique : Equivalence class (happy path)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetAvailable_ReturnsOkWithAvailableBooks()
        {
            // Arrange
            var books = new List<BookDto>
            {
                new BookDto { Id = 1, Title = "Dune",    IsAvailable = true },
                new BookDto { Id = 3, Title = "Hobbit",  IsAvailable = true }
            };
            _repoMock.Setup(r => r.GetAvailableBooksAsync()).ReturnsAsync(books);

            // Act
            var result = await _sut.GetAvailable();

            // Assert
            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            var list = ok.Value.Should().BeAssignableTo<List<BookDto>>().Subject;
            list.Should().HaveCount(2).And.OnlyContain(b => b.IsAvailable);
        }
    }
}
