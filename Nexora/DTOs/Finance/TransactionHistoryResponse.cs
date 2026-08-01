namespace Nexora.DTOs.Finance;

public class TransactionHistoryResponse
{
    public required string SenderName { get; set; }
    public required string ReceiverName { get; set; }
    public decimal Amount { get; set; }
    public DateTime Date { get; set; }
    public required string Currency { get; set; }
}