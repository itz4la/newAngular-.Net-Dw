using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Book
    {
    public class CreateBookDto
        {
        [Required(ErrorMessage = "Title is required")]
        [StringLength(150, ErrorMessage = "Title cannot exceed 150 characters")]
        public string Title { get; set; }

        [Required(ErrorMessage = "Author is required")]
        [StringLength(100, ErrorMessage = "Author name cannot exceed 100 characters")]
        public string Author { get; set; }

        [StringLength(5000, ErrorMessage = "Description cannot exceed 5000 characters")]
        public string Description { get; set; }

        [Required(ErrorMessage = "Genre is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Please select a valid genre")]
        public int GenreId { get; set; }

        [StringLength(255, ErrorMessage = "Cover image URL cannot exceed 255 characters")]
        [Url(ErrorMessage = "Please provide a valid URL")]
        public string CoverImageUrl { get; set; }

        [Required(ErrorMessage = "Published date is required")]
        public DateTime PublishedDate { get; set; }
        }
    }
