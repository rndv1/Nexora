namespace Nexora.Models
{
    public class Session
    {
        public int UserId { get; set; }
        public string Token { get; set; }
        public DateTime ExpiresAt { get; set; }
    }
}
