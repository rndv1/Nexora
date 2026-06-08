using System.ComponentModel.DataAnnotations;

namespace Nexora.DTOs.User
{
    public class LoginRequest
    {
        [Required(ErrorMessage = "Поле Login обязательно")]
        [MinLength(3, ErrorMessage = "Минимальная длина логина 6 символов")]
        public required string Login { get; set; }

        [Required(ErrorMessage = "Поле Password обязательно")]
        [MinLength(6, ErrorMessage = "Минимальная длина пароля 6 символов")]
        public required string PasswordHash { get; set; }
    }
}
