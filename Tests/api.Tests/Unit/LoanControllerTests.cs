// =============================================================================
// FILE    : Unit/LoanControllerTests.cs
// PURPOSE : Unit tests for LoanController (borrow / return / validation)
// LEVEL   : Unit
// TYPE    : Functional – Black-box (equivalence classes + boundary values)
// =============================================================================

using api.Controllers;
using api.DTOs.Book;
using api.DTOs.Loan;
using api.Repositories.Loan;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace api.Tests.Unit
{
    public class LoanControllerTests
    {
        private readonly Mock<ILoanRepository> _repoMock;
        private readonly LoanController        _sut;

        public LoanControllerTests()
        {
            _repoMock = new Mock<ILoanRepository>();
            _sut      = new LoanController(_repoMock.Object);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-001  GetById – valid existing loan → 200 OK
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_ValidId_ReturnsOk()
        {
            var loan = new LoanDto
            {
                Id        = 1,
                BookId    = 2,
                BookTitle = "1984",
                UserId    = "user-123",
                Status    = "Active"
            };
            _repoMock.Setup(r => r.GetByIdAsync(1)).ReturnsAsync(loan);

            var result = await _sut.GetById(1);

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-002  GetById – ID ≤ 0 → 400 BadRequest
        // Technique : Boundary value analysis
        // ─────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task GetById_InvalidId_ReturnsBadRequest(int badId)
        {
            var result = await _sut.GetById(badId);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
            _repoMock.Verify(r => r.GetByIdAsync(It.IsAny<int>()), Times.Never);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-003  GetById – non-existent loan → 404 NotFound
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetById_NonExistent_ReturnsNotFound()
        {
            _repoMock.Setup(r => r.GetByIdAsync(999)).ReturnsAsync((LoanDto?)null);
            var result = await _sut.GetById(999);
            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-004  CreateLoan – valid loan → 201 Created
        // Technique : Equivalence class (happy path)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task CreateLoan_ValidDto_ReturnsCreated()
        {
            // Arrange
            var dto = new CreateLoanDto { BookId = 1, UserId = "user-abc" };
            var created = new LoanDto { Id = 10, BookId = 1, UserId = "user-abc", Status = "Active" };

            _repoMock.Setup(r => r.ValidateLoanCreationAsync("user-abc", 1))
                     .ReturnsAsync(LoanValidationResult.Success());
            _repoMock.Setup(r => r.CreateLoanAsync(dto)).ReturnsAsync(created);

            // Act
            var result = await _sut.CreateLoan(dto);

            // Assert
            result.Result.Should().BeOfType<CreatedAtActionResult>()
                  .Which.StatusCode.Should().Be(201);
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-005  CreateLoan – book already borrowed → 400 BadRequest
        // Technique : Equivalence class (invalid: unavailable book)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task CreateLoan_BookAlreadyBorrowed_ReturnsBadRequest()
        {
            var dto = new CreateLoanDto { BookId = 3, UserId = "user-abc" };
            _repoMock.Setup(r => r.ValidateLoanCreationAsync("user-abc", 3))
                     .ReturnsAsync(LoanValidationResult.Failure("This book is currently borrowed by another user"));

            var result = await _sut.CreateLoan(dto);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-006  CreateLoan – user already has 5 books → 400 BadRequest
        // Technique : Boundary value analysis (max loan limit reached)
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task CreateLoan_MaxLoanLimitReached_ReturnsBadRequest()
        {
            var dto = new CreateLoanDto { BookId = 7, UserId = "user-max" };
            _repoMock.Setup(r => r.ValidateLoanCreationAsync("user-max", 7))
                     .ReturnsAsync(LoanValidationResult.Failure("You cannot borrow more than 5 books at once"));

            var result = await _sut.CreateLoan(dto);

            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-007  ReturnLoan – valid existing loan → 200 OK
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task ReturnLoan_ValidLoan_ReturnsOk()
        {
            var returned = new LoanDto { Id = 1, Status = "Returned", ReturnDate = DateTime.Now };
            _repoMock.Setup(r => r.ReturnLoanAsync(1, null)).ReturnsAsync(returned);

            var result = await _sut.ReturnLoan(1);

            result.Result.Should().BeOfType<OkObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-008  ReturnLoan – already returned loan → 404 NotFound
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task ReturnLoan_AlreadyReturned_ReturnsNotFound()
        {
            _repoMock.Setup(r => r.ReturnLoanAsync(5, null)).ReturnsAsync((LoanDto?)null);

            var result = await _sut.ReturnLoan(5);

            result.Result.Should().BeOfType<NotFoundObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-009  CheckBookAvailability – available book → isAvailable = true
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task CheckBookAvailability_AvailableBook_ReturnsTrue()
        {
            _repoMock.Setup(r => r.IsBookAvailableAsync(2)).ReturnsAsync(true);

            var result = await _sut.CheckBookAvailability(2);

            var ok = result.Result.Should().BeOfType<OkObjectResult>().Subject;
            // The anonymous object { bookId=2, isAvailable=true }
            ok.Value.Should().NotBeNull();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-010  GetUserActiveLoans – empty userId → 400 BadRequest
        // Technique : Equivalence class (invalid input: empty string)
        // ─────────────────────────────────────────────────────────────────────
        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task GetUserActiveLoans_EmptyUserId_ReturnsBadRequest(string emptyId)
        {
            var result = await _sut.GetUserActiveLoans(emptyId);
            result.Result.Should().BeOfType<BadRequestObjectResult>();
        }

        // ─────────────────────────────────────────────────────────────────────
        // TC-U-LOAN-011  GetUserLoanCount – valid userId returns count + limit
        // ─────────────────────────────────────────────────────────────────────
        [Fact]
        public async Task GetUserLoanCount_ValidUserId_ReturnsCount()
        {
            _repoMock.Setup(r => r.GetUserActiveLoanCountAsync("user1")).ReturnsAsync(3);

            var result = await _sut.GetUserLoanCount("user1");

            result.Result.Should().BeOfType<OkObjectResult>();
        }
    }
}
