namespace api.DTOs.Book
    {
    public class BookDto
        {
        public int Id { get; set; }
        public string Title { get; set; }
        public string Author { get; set; }
        public string Description { get; set; }
        public int GenreId { get; set; }
        public string GenreName { get; set; }
        public string CoverImageUrl { get; set; }
        public DateTime PublishedDate { get; set; }
        public bool IsAvailable { get; set; }
        }
    }
