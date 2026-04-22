using System.ComponentModel.DataAnnotations;

namespace SmartEventBooking.Application.DTOs.Auth
{
    public class LoginDto
    {
        [Required]
        [EmailAddress]
        public required string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public required string Password { get; set; }

        [Required]
        public bool RememberMe { get; set; }
    }
}