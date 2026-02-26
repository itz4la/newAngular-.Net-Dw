namespace api.DTOs.User
    {
    public class UserFilterDto
        {
        public string? UserName { get; set; }
        public string? Role { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        }
    }
