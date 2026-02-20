using api.DTOs.Book;
using api.DTOs.Loan;
using api.Repositories.Loan;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace api.Controllers
    {
    [Route("api/[controller]")]
    [ApiController]
    public class LoanController : ControllerBase
        {
        private readonly ILoanRepository _loanRepository;

        public LoanController(ILoanRepository loanRepository)
            {
            _loanRepository = loanRepository;
            }

        [HttpGet("{id}")]
        public async Task<ActionResult<LoanDto>> GetById(int id)
            {
            if (id <= 0)
                return BadRequest("Invalid loan ID");

            var loan = await _loanRepository.GetByIdAsync(id);

            if (loan == null)
                return NotFound($"Loan with ID {id} not found");

            return Ok(loan);
            }

        [HttpGet]
        public async Task<ActionResult<PagedResultDto<LoanDto>>> GetAll([FromQuery] LoanFilterDto filter)
            {
            var result = await _loanRepository.GetAllAsync(filter);
            return Ok(result);
            }

        [HttpGet("user/{userId}/active")]
        public async Task<ActionResult<List<LoanDto>>> GetUserActiveLoans(string userId)
            {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User ID is required");

            var loans = await _loanRepository.GetUserActiveLoansAsync(userId);
            return Ok(loans);
            }

        [HttpGet("book/{bookId}/availability")]
        public async Task<ActionResult<object>> CheckBookAvailability(int bookId)
            {
            if (bookId <= 0)
                return BadRequest("Invalid book ID");

            var isAvailable = await _loanRepository.IsBookAvailableAsync(bookId);
            return Ok(new { bookId, isAvailable });
            }

        [HttpGet("user/{userId}/loan-count")]
        public async Task<ActionResult<object>> GetUserLoanCount(string userId)
            {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest("User ID is required");

            var count = await _loanRepository.GetUserActiveLoanCountAsync(userId);
            return Ok(new { userId, activeLoanCount = count, maxAllowed = 5 });
            }

        [HttpPost]
        public async Task<ActionResult<LoanDto>> CreateLoan([FromBody] CreateLoanDto createLoanDto)
            {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var validation = await _loanRepository.ValidateLoanCreationAsync(createLoanDto.UserId, createLoanDto.BookId);
            if (!validation.IsValid)
                return BadRequest(validation.ErrorMessage);

            var loan = await _loanRepository.CreateLoanAsync(createLoanDto);

            if (loan == null)
                return BadRequest("Unable to create loan. Please check the validation rules");

            return CreatedAtAction(nameof(GetById), new { id = loan.Id }, loan);
            }

        [HttpPost("return/{loanId}")]
        public async Task<ActionResult<LoanDto>> ReturnLoan(int loanId, [FromBody] ReturnLoanDto returnLoanDto = null)
            {
            if (loanId <= 0)
                return BadRequest("Invalid loan ID");

            var loan = await _loanRepository.ReturnLoanAsync(loanId, returnLoanDto?.ReturnDate);

            if (loan == null)
                return NotFound("Loan not found or already returned");

            return Ok(loan);
            }

        [HttpPost("update-overdue")]
        public async Task<ActionResult> UpdateOverdueLoans()
            {
            await _loanRepository.UpdateOverdueLoansAsync();
            return Ok(new { message = "Overdue loans updated successfully" });
            }
        }
    }
