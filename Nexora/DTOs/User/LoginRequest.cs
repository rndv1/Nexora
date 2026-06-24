using System.ComponentModel.DataAnnotations;

namespace Nexora.DTOs.User
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Login is required")]
        [MinLength(4, ErrorMessage = "Login must be at least 4 characters long")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public required string PasswordHash { get; set; }
    }
}
