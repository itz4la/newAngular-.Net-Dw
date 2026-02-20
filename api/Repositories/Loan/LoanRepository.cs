using api.DTOs.Book;
using api.DTOs.Loan;
using api.models;
using Microsoft.EntityFrameworkCore;

namespace api.Repositories.Loan
    {
    public class LoanRepository : ILoanRepository
        {
        private readonly ApplicationContext _context;
        private const int MAX_BOOKS_PER_USER = 5;

        public LoanRepository(ApplicationContext context)
            {
            _context = context;
            }

        public async Task<LoanDto> GetByIdAsync(int id)
            {
            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .FirstOrDefaultAsync(l => l.Id == id);

            if (loan == null)
                return null;

            return MapToDto(loan);
            }

        public async Task<PagedResultDto<LoanDto>> GetAllAsync(LoanFilterDto filter)
            {
            var query = _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.UserId))
                {
                query = query.Where(l => l.UserId == filter.UserId);
                }

            if (filter.BookId.HasValue)
                {
                query = query.Where(l => l.BookId == filter.BookId.Value);
                }

            if (!string.IsNullOrWhiteSpace(filter.Status))
                {
                if (Enum.TryParse<LoanStatus>(filter.Status, true, out var status))
                    {
                    query = query.Where(l => l.Status == status);
                    }
                }

            if (filter.IsOverdue.HasValue && filter.IsOverdue.Value)
                {
                query = query.Where(l => l.Status == LoanStatus.Overdue);
                }

            var totalCount = await query.CountAsync();

            if (filter.PageSize <= 0)
                filter.PageSize = 10;
            if (filter.PageNumber <= 0)
                filter.PageNumber = 1;

            var loans = await query
                .OrderByDescending(l => l.LoanDate)
                .Skip((filter.PageNumber - 1) * filter.PageSize)
                .Take(filter.PageSize)
                .Select(l => MapToDto(l))
                .ToListAsync();

            return new PagedResultDto<LoanDto>
                {
                Items = loans,
                TotalCount = totalCount,
                PageNumber = filter.PageNumber,
                PageSize = filter.PageSize
                };
            }

        public async Task<List<LoanDto>> GetUserActiveLoansAsync(string userId)
            {
            var loans = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.User)
                .Where(l => l.UserId == userId && l.Status == LoanStatus.Active)
                .OrderBy(l => l.DueDate)
                .Select(l => MapToDto(l))
                .ToListAsync();

            return loans;
            }

        public async Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto)
            {
            var validation = await ValidateLoanCreationAsync(createLoanDto.UserId, createLoanDto.BookId);
            if (!validation.IsValid)
                return null;

            var loan = new models.Loan
                {
                BookId = createLoanDto.BookId,
                UserId = createLoanDto.UserId,
                LoanDate = DateTime.Now,
                DueDate = createLoanDto.CustomDueDate ?? DateTime.Now.AddDays(14),
                Status = LoanStatus.Active
                };

            _context.Loans.Add(loan);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(loan.Id);
            }

        public async Task<LoanDto> ReturnLoanAsync(int loanId, DateTime? returnDate = null)
            {
            var loan = await _context.Loans.FindAsync(loanId);
            if (loan == null)
                return null;

            if (loan.Status == LoanStatus.Returned)
                return null;

            loan.ReturnDate = returnDate ?? DateTime.Now;
            loan.Status = LoanStatus.Returned;

            _context.Loans.Update(loan);
            await _context.SaveChangesAsync();

            return await GetByIdAsync(loan.Id);
            }

        public async Task<LoanValidationResult> ValidateLoanCreationAsync(string userId, int bookId)
            {
            var bookExists = await _context.Books.AnyAsync(b => b.Id == bookId);
            if (!bookExists)
                return LoanValidationResult.Failure("Book not found");

            var userExists = await _context.Users.AnyAsync(u => u.Id == userId);
            if (!userExists)
                return LoanValidationResult.Failure("User not found");

            var isBookAvailable = await IsBookAvailableAsync(bookId);
            if (!isBookAvailable)
                return LoanValidationResult.Failure("This book is currently borrowed by another user");

            var userActiveLoanCount = await GetUserActiveLoanCountAsync(userId);
            if (userActiveLoanCount >= MAX_BOOKS_PER_USER)
                return LoanValidationResult.Failure($"You cannot borrow more than {MAX_BOOKS_PER_USER} books at once");

            return LoanValidationResult.Success();
            }

        public async Task<bool> IsBookAvailableAsync(int bookId)
            {
            var activeLoans = await _context.Loans
                .Where(l => l.BookId == bookId && (l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue))
                .AnyAsync();

            return !activeLoans;
            }

        public async Task<int> GetUserActiveLoanCountAsync(string userId)
            {
            return await _context.Loans
                .Where(l => l.UserId == userId && (l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue))
                .CountAsync();
            }

        public async Task<bool> HasUserBorrowedBookAsync(string userId, int bookId)
            {
            return await _context.Loans
                .AnyAsync(l => l.UserId == userId && l.BookId == bookId && (l.Status == LoanStatus.Active || l.Status == LoanStatus.Overdue));
            }

        public async Task UpdateOverdueLoansAsync()
            {
            var overdueLoans = await _context.Loans
                .Where(l => l.Status == LoanStatus.Active && l.DueDate < DateTime.Now)
                .ToListAsync();

            foreach (var loan in overdueLoans)
                {
                loan.Status = LoanStatus.Overdue;
                }

            if (overdueLoans.Any())
                {
                await _context.SaveChangesAsync();
                }
            }

        private static LoanDto MapToDto(models.Loan loan)
            {
            var daysRemaining = (loan.DueDate - DateTime.Now).Days;
            var isOverdue = loan.Status == LoanStatus.Overdue || (loan.Status == LoanStatus.Active && DateTime.Now > loan.DueDate);

            return new LoanDto
                {
                Id = loan.Id,
                BookId = loan.BookId,
                BookTitle = loan.Book?.Title,
                UserId = loan.UserId,
                UserName = loan.User?.UserName,
                LoanDate = loan.LoanDate,
                DueDate = loan.DueDate,
                ReturnDate = loan.ReturnDate,
                Status = loan.Status.ToString(),
                DaysRemaining = daysRemaining,
                IsOverdue = isOverdue
                };
            }
        }
    }
