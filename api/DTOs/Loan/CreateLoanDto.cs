using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Loan
    {
    public class CreateLoanDto
        {
        [Required(ErrorMessage = "Book ID is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please provide a valid book ID")]
        public int BookId { get; set; }

        [Required(ErrorMessage = "User ID is required")]
        public string UserId { get; set; }

        public DateTime? CustomDueDate { get; set; }
        }
    }
