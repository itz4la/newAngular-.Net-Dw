using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace api.models

    {
    public class Book
        {

        [Key]
        public int Id { get; set; }

        [Required]
        [StringLength(150)]
        [MaxLength(150, ErrorMessage = "Title is too long")]
        public string Title { get; set; }

        [Required]
        [StringLength(100)]
        [MaxLength(100, ErrorMessage = "Author name is too long")]
        public string Author { get; set; }


        [Column(TypeName = "text")]
        public string Description { get; set; }

        // 🔗 Foreign Key -> Genre
        [Required]
        public int GenreId { get; set; }

        [ForeignKey(nameof(GenreId))]
        public Genre Genre { get; set; }

        [StringLength(255)]
        [MaxLength(255, ErrorMessage = "Cover image URL is too long")]
        public string CoverImageUrl { get; set; }


        [Required]
        public DateTime PublishedDate { get; set; }


        }
    }
