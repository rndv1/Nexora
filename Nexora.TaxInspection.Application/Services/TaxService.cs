using Microsoft.Extensions.Logging;
using Nexora.TaxInspection.Application.Interfaces;
using Nexora.TaxInspection.Domain.Events;

namespace Nexora.TaxInspection.Application.Services;

public class TaxService : ITaxService
{
    private readonly ILogger<TaxService> _logger;

    public TaxService(ILogger<TaxService> logger)
    {
        _logger = logger;
    }

    public Task ProcessTransactionAsync(TransactionCreatedEvent transactionEvent, CancellationToken cancellationToken)
    {
        var tax = transactionEvent.Amount * 0.13m;
        
        _logger.LogInformation(
            "Processed transaction from {SenderId} to {ReceiverId}. Amount: {Amount} {Currency}. Tax calculated: {Tax} {Currency}",
            transactionEvent.SenderId,
            transactionEvent.ReceiverId,
            transactionEvent.Amount,
            transactionEvent.Currency,
            tax,
            transactionEvent.Currency);

        return Task.CompletedTask;
    }
}
