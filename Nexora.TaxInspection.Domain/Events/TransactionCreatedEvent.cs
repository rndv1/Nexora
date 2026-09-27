namespace Nexora.TaxInspection.Domain.Events;

public class TransactionCreatedEvent
{
    public int SenderId { get; set; }
    public int ReceiverId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; } = string.Empty;
}
