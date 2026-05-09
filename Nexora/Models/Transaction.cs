namespace Nexora.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int SenderAccountId { get; set; }
        public int ReceiverAccountId { get; set; }
        public int SenderUserId { get; set; }
        public int ReceiverUserId { get; set; }
        public DateTime CreatedAt { get; set; }
        public decimal Amount { get; set; }
        public User? Sender { get; set; }
        public User? Receiver { get; set; }
    }
}
