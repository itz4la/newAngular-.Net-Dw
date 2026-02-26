using System.ComponentModel.DataAnnotations;

namespace api.DTOs.User
    {
    public class UpdateUserDto
        {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        public string Role { get; set; }
        }
    }
