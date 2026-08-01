namespace Nexora.DTOs.Finance;

public class BalanceResponse
{
    public decimal Balance { get; set; }
    public string Currency { get; set; } = string.Empty;
}