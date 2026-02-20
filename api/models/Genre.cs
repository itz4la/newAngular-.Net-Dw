using System.ComponentModel.DataAnnotations;

namespace api.models
    {
    public class Genre
        {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(50)]
        [MaxLength(50, ErrorMessage = "Genre name is too long")]
        public string Name { get; set; }

        // Navigation Property (One Genre -> Many Books)
        public ICollection<Book> Books { get; set; } = new List<Book>();

        }
    }
