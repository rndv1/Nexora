namespace Nexora.Models
{
    public class Session
    {
        public int UserId { get; set; }
        public string Token { get; set; } = string.Empty;
        public DateTime ExpiresAt { get; set; }
        public User? User { get; set; }
    }
}
