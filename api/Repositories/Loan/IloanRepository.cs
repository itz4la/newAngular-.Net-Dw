using api.DTOs.Book;
using api.DTOs.Loan;

namespace api.Repositories.Loan
    {
    public interface ILoanRepository
        {
        Task<LoanDto> GetByIdAsync(int id);
        Task<PagedResultDto<LoanDto>> GetAllAsync(LoanFilterDto filter);
        Task<List<LoanDto>> GetUserActiveLoansAsync(string userId);
        Task<LoanDto> CreateLoanAsync(CreateLoanDto createLoanDto);
        Task<LoanDto> ReturnLoanAsync(int loanId, DateTime? returnDate = null);
        Task<LoanValidationResult> ValidateLoanCreationAsync(string userId, int bookId);
        Task<bool> IsBookAvailableAsync(int bookId);
        Task<int> GetUserActiveLoanCountAsync(string userId);
        Task<bool> HasUserBorrowedBookAsync(string userId, int bookId);
        Task UpdateOverdueLoansAsync();
        }
    }
