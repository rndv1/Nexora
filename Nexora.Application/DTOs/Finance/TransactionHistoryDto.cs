namespace Nexora.Application.DTOs.Finance;

public class TransactionHistoryDto
{
    public DateTime Date { get; set; }
    public decimal Amount { get; set; }
    public string SenderName { get; set; } = string.Empty;
    public string ReceiverName { get; set; } = string.Empty;
    public string Currency { get; set; } = string.Empty;
}
