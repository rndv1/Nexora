namespace Nexora.DTOs
{
    public class RegisterRequest
    {
        public string Login { get; set; }
        public string Name { get; set; }
        public string PasswordHash { get; set; }
    }
}
