namespace Nexora.DTOs.Finance
{
    public class TransferRequest
    {
        public string? ReceiverLogin { get; set; }
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }
}