namespace Nexora.DTOs.Finance
{
    public class TransactionHistoryRequest
    {
        public DateTime? From { get; set; }
        public DateTime? To { get; set; }
        public int Limit { get; set; } = 20;
        public int Offset { get; set; } = 0;
    }
}