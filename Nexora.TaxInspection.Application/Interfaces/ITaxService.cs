using Nexora.TaxInspection.Domain.Events;

namespace Nexora.TaxInspection.Application.Interfaces;

public interface ITaxService
{
    Task ProcessTransactionAsync(TransactionCreatedEvent transactionEvent, CancellationToken cancellationToken);
}
