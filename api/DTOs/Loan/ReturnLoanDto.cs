using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Loan
    {
    public class ReturnLoanDto
        {
        [Required(ErrorMessage = "Loan ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid loan ID")]
        public int LoanId { get; set; }

        public DateTime? ReturnDate { get; set; }
        }
    }
