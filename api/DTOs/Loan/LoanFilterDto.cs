namespace api.DTOs.Loan
    {
    public class LoanFilterDto
        {
        public string? UserId { get; set; }
        public string? UserName { get; set; }
        public int? BookId { get; set; }
        public string? Status { get; set; }
        public bool? IsOverdue { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        }
    }
