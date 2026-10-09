using System.Text.Json;
using Nexora.Application.Events;
using Nexora.Application.Interfaces;

namespace Nexora.API.BackgroundServices;

public class OutboxPublisherBackgroundService : BackgroundService
{
    private static readonly TimeSpan PollingInterval = TimeSpan.FromSeconds(2);
    private const int BatchSize = 20;

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<OutboxPublisherBackgroundService> _logger;

    public OutboxPublisherBackgroundService(
        IServiceScopeFactory scopeFactory,
        ILogger<OutboxPublisherBackgroundService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("OutboxPublisherBackgroundService started");

        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                await ProcessOutboxBatchAsync(stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error in OutboxPublisherBackgroundService");
            }

            try
            {
                await Task.Delay(PollingInterval, stoppingToken);
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }

        _logger.LogInformation("OutboxPublisherBackgroundService stopped");
    }

    private async Task ProcessOutboxBatchAsync(CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var outboxRepository = scope.ServiceProvider.GetRequiredService<IOutboxRepository>();
        var messageProducer = scope.ServiceProvider.GetRequiredService<IMessageProducer>();

        var messages = await outboxRepository.GetUnprocessedMessagesAsync(BatchSize, cancellationToken);
        if (messages.Count == 0)
        {
            return;
        }

        foreach (var message in messages)
        {
            try
            {
                if (message.Type == nameof(TransactionCreatedEvent))
                {
                    var @event = JsonSerializer.Deserialize<TransactionCreatedEvent>(message.Payload);
                    if (@event != null)
                    {
                        await messageProducer.SendMessageAsync(@event, cancellationToken);
                    }
                }
                else
                {
                    _logger.LogWarning("Unknown OutboxMessage type {MessageType}", message.Type);
                }

                message.ProcessedAt = DateTime.UtcNow;
                message.Error = null;
                _logger.LogInformation("Successfully published OutboxMessage {MessageId} ({MessageType})", message.Id, message.Type);
            }
            catch (Exception ex)
            {
                message.RetryCount++;
                message.Error = ex.Message;
                _logger.LogError(ex, "Failed to publish OutboxMessage {MessageId}. Attempt {RetryCount}", message.Id, message.RetryCount);
            }
        }

        await outboxRepository.SaveChangesAsync(cancellationToken);
    }
}
