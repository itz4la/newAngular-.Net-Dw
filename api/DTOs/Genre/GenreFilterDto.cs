namespace api.DTOs.Genre
    {
    public class GenreFilterDto
        {
        public string? Name { get; set; }
        public int PageNumber { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        }
    }
