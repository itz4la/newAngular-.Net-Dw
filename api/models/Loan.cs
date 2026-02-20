using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.models
    {

    public enum LoanStatus
        {
        Active = 1,
        Returned = 2,
        Overdue = 3
        }

    public class Loan
        {
        [Key]
        public int Id { get; set; }

        [Required]
        public int BookId { get; set; }

        [ForeignKey(nameof(BookId))]
        public Book Book { get; set; }
   
        [Required]
        public string UserId { get; set; }

        [ForeignKey(nameof(UserId))]
        public ApplicationUser User { get; set; }

        [Required]
        public DateTime LoanDate { get; set; } = DateTime.Now;

        [Required]
        public DateTime DueDate { get; set; } = DateTime.Now.AddDays(14);

        public DateTime? ReturnDate { get; set; }

        [Required]
        public LoanStatus Status { get; set; } = LoanStatus.Active;

        }
    }
