namespace api.DTOs.Loan
    {
    public class LoanDto
        {
        public int Id { get; set; }
        public int BookId { get; set; }
        public string BookTitle { get; set; }
        public string UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LoanDate { get; set; }
        public DateTime DueDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string Status { get; set; }
        public int DaysRemaining { get; set; }
        public bool IsOverdue { get; set; }
        }
    }
