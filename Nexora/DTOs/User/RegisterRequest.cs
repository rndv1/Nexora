using System.ComponentModel.DataAnnotations;

namespace Nexora.DTOs.User
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Login is required")]
        [MinLength(4, ErrorMessage = "Login must be at least 4 characters long")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Name is required")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [MinLength(6, ErrorMessage = "Password must be at least 6 characters long")]
        public required string PasswordHash { get; set; }
    }
}
