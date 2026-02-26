using System.ComponentModel.DataAnnotations;

namespace api.DTOs.User
    {
    public class CreateAdminUserDto
        {
        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        public string Password { get; set; }
        }
    }
