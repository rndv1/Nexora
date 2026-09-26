namespace Nexora.API.DTOs.User
{
    public class LoginRequest
    {
        public string? Login { get; set; }
        public string? PasswordHash { get; set; }
    }
}
