using System.ComponentModel.DataAnnotations;

namespace api.DTOs.Genre
    {
    public class UpdateGenreDto
        {
        [Required(ErrorMessage = "Genre name is required")]
        [StringLength(50, ErrorMessage = "Genre name cannot exceed 50 characters")]
        public string Name { get; set; }
        }
    }
