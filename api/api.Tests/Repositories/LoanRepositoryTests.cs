using api.DTOs.Loan;
using api.models;
using api.Repositories.Loan;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace api.Tests.Repositories;

public class LoanRepositoryTests
{
    private static ApplicationContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<ApplicationContext>()
            .UseInMemoryDatabase(Guid.NewGuid().ToString())
            .Options;

        return new ApplicationContext(options);
    }

    private static async Task SeedBasicDataAsync(ApplicationContext context, string userId = "user-1", int bookId = 1)
    {
        var genre = new Genre { Id = 1, Name = "Sci-Fi" };
        var user = new ApplicationUser { Id = userId, UserName = "reader" };
        var book = new Book
        {
            Id = bookId,
            Title = "Dune",
            Author = "Frank Herbert",
            Description = "Classic",
            CoverImageUrl = "https://example.com/dune.jpg",
            GenreId = genre.Id,
            Genre = genre,
            PublishedDate = new DateTime(1965, 1, 1)
        };

        context.Genres.Add(genre);
        context.Users.Add(user);
        context.Books.Add(book);

        await context.SaveChangesAsync();
    }

    [Fact]
    public async Task GetByIdAsync_ReturnsMappedLoan_WhenLoanExists()
    {
        await using var context = CreateContext();
        await SeedBasicDataAsync(context);

        var loan = new Loan
        {
            Id = 10,
            BookId = 1,
            UserId = "user-1",
            LoanDate = DateTime.UtcNow.AddDays(-2),
            DueDate = DateTime.UtcNow.AddDays(12),
            Status = LoanStatus.Active
        };

        context.Loans.Add(loan);
        await context.SaveChangesAsync();

        var repository = new LoanRepository(context);

        var result = await repository.GetByIdAsync(10);

        Assert.NotNull(result);
        Assert.Equal(10, result.Id);
        Assert.Equal("Dune", result.BookTitle);
        Assert.Equal("reader", result.UserName);
        Assert.Equal("Active", result.Status);
    }

    [Fact]
    public async Task CreateLoanAsync_CreatesLoan_WhenValidationPasses()
    {
        await using var context = CreateContext();
        await SeedBasicDataAsync(context);
        var repository = new LoanRepository(context);

        var createDto = new CreateLoanDto
        {
            BookId = 1,
            UserId = "user-1"
        };

        var result = await repository.CreateLoanAsync(createDto);

        Assert.NotNull(result);
        Assert.Equal("Active", result.Status);
        Assert.Equal(1, await context.Loans.CountAsync());
    }

    [Fact]
    public async Task CreateLoanAsync_ReturnsNull_WhenBookDoesNotExist()
    {
        await using var context = CreateContext();
        context.Users.Add(new ApplicationUser { Id = "user-1", UserName = "reader" });
        await context.SaveChangesAsync();

        var repository = new LoanRepository(context);

        var result = await repository.CreateLoanAsync(new CreateLoanDto
        {
            BookId = 999,
            UserId = "user-1"
        });

        Assert.Null(result);
    }

    [Fact]
    public async Task ReturnLoanAsync_SetsReturnedStatus_AndReturnDate()
    {
        await using var context = CreateContext();
        await SeedBasicDataAsync(context);

        context.Loans.Add(new Loan
        {
            Id = 1,
            BookId = 1,
            UserId = "user-1",
            LoanDate = DateTime.UtcNow.AddDays(-5),
            DueDate = DateTime.UtcNow.AddDays(9),
            Status = LoanStatus.Active
        });
        await context.SaveChangesAsync();

        var repository = new LoanRepository(context);
        var returnDate = new DateTime(2026, 1, 1);

        var result = await repository.ReturnLoanAsync(1, returnDate);

        Assert.NotNull(result);
        Assert.Equal("Returned", result.Status);
        Assert.Equal(returnDate, result.ReturnDate);
    }

    [Fact]
    public async Task ValidateLoanCreationAsync_Fails_WhenUserReachedMaxBooks()
    {
        await using var context = CreateContext();
        await SeedBasicDataAsync(context);

        var extraBooks = Enumerable.Range(2, 5)
            .Select(id => new Book
            {
                Id = id,
                Title = $"Book {id}",
                Author = "Author",
                Description = "Desc",
                CoverImageUrl = $"https://example.com/book-{id}.jpg",
                GenreId = 1,
                PublishedDate = new DateTime(2000, 1, 1)
            });

        context.Books.AddRange(extraBooks);
        await context.SaveChangesAsync();

        for (var i = 1; i <= 5; i++)
        {
            context.Loans.Add(new Loan
            {
                BookId = i,
                UserId = "user-1",
                LoanDate = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(13),
                Status = LoanStatus.Active
            });
        }

        await context.SaveChangesAsync();

        var repository = new LoanRepository(context);

        var result = await repository.ValidateLoanCreationAsync("user-1", 6);

        Assert.False(result.IsValid);
        Assert.Contains("cannot borrow more than", result.ErrorMessage, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public async Task UpdateOverdueLoansAsync_MarksActivePastDueLoansAsOverdue()
    {
        await using var context = CreateContext();
        await SeedBasicDataAsync(context);

        context.Loans.Add(new Loan
        {
            Id = 7,
            BookId = 1,
            UserId = "user-1",
            LoanDate = DateTime.UtcNow.AddDays(-20),
            DueDate = DateTime.UtcNow.AddDays(-1),
            Status = LoanStatus.Active
        });
        await context.SaveChangesAsync();

        var repository = new LoanRepository(context);

        await repository.UpdateOverdueLoansAsync();

        var updated = await context.Loans.FindAsync(7);
        Assert.NotNull(updated);
        Assert.Equal(LoanStatus.Overdue, updated.Status);
    }

    [Fact]
    public async Task GetAllAsync_AppliesStatusFilterAndPaging()
    {
        await using var context = CreateContext();
        await SeedBasicDataAsync(context);

        context.Loans.AddRange(
            new Loan
            {
                BookId = 1,
                UserId = "user-1",
                LoanDate = DateTime.UtcNow.AddDays(-1),
                DueDate = DateTime.UtcNow.AddDays(13),
                Status = LoanStatus.Active
            },
            new Loan
            {
                BookId = 1,
                UserId = "user-1",
                LoanDate = DateTime.UtcNow.AddDays(-2),
                DueDate = DateTime.UtcNow.AddDays(12),
                Status = LoanStatus.Returned,
                ReturnDate = DateTime.UtcNow.AddDays(-1)
            });
        await context.SaveChangesAsync();

        var repository = new LoanRepository(context);

        var result = await repository.GetAllAsync(new LoanFilterDto
        {
            Status = "Active",
            PageNumber = 1,
            PageSize = 10
        });

        Assert.Single(result.Items);
        Assert.Equal(1, result.TotalCount);
        Assert.Equal("Active", result.Items[0].Status);
    }
}
