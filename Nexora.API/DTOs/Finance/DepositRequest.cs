namespace Nexora.API.DTOs.Finance
{
    public class DepositRequest
    {
        public decimal Amount { get; set; }
        public string? Currency { get; set; }
    }
}
