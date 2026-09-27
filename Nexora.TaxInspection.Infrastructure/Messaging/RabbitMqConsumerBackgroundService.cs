using System.Text;
using System.Text.Json;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.DependencyInjection;
using Nexora.TaxInspection.Application.Interfaces;
using Nexora.TaxInspection.Domain.Events;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace Nexora.TaxInspection.Infrastructure.Messaging;

public class RabbitMqConsumerBackgroundService : BackgroundService
{
    private readonly RabbitMqSettings _settings;
    private readonly ILogger<RabbitMqConsumerBackgroundService> _logger;
    private readonly IServiceScopeFactory _scopeFactory;
    private IConnection? _connection;
    private IChannel? _channel;

    public RabbitMqConsumerBackgroundService(
        IOptions<RabbitMqSettings> settings,
        ILogger<RabbitMqConsumerBackgroundService> logger,
        IServiceScopeFactory scopeFactory)
    {
        _settings = settings.Value;
        _logger = logger;
        _scopeFactory = scopeFactory;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var factory = new ConnectionFactory
        {
            HostName = _settings.HostName,
            UserName = _settings.UserName,
            Password = _settings.Password
        };

        try
        {
            _connection = await factory.CreateConnectionAsync(stoppingToken);
            _channel = await _connection.CreateChannelAsync(cancellationToken: stoppingToken);

            var queueName = nameof(TransactionCreatedEvent);
            
            await _channel.QueueDeclareAsync(
                queue: queueName,
                durable: true,
                exclusive: false,
                autoDelete: false,
                arguments: null,
                cancellationToken: stoppingToken);

            var consumer = new AsyncEventingBasicConsumer(_channel);
            consumer.ReceivedAsync += async (model, ea) =>
            {
                try
                {
                    var body = ea.Body.ToArray();
                    var json = Encoding.UTF8.GetString(body);
                    var transactionEvent = JsonSerializer.Deserialize<TransactionCreatedEvent>(json);

                    if (transactionEvent != null)
                    {
                        using var scope = _scopeFactory.CreateScope();
                        var taxService = scope.ServiceProvider.GetRequiredService<ITaxService>();
                        await taxService.ProcessTransactionAsync(transactionEvent, stoppingToken);
                    }

                    await _channel.BasicAckAsync(ea.DeliveryTag, multiple: false, cancellationToken: stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error processing message");
                    await _channel.BasicNackAsync(ea.DeliveryTag, multiple: false, requeue: true, cancellationToken: stoppingToken);
                }
            };

            await _channel.BasicConsumeAsync(queue: queueName, autoAck: false, consumer: consumer, cancellationToken: stoppingToken);
            
            _logger.LogInformation("TaxInspection RabbitMQ Consumer started listening on {Queue}", queueName);
            
            await Task.Delay(Timeout.Infinite, stoppingToken);
        }
        catch (OperationCanceledException)
        {
            _logger.LogInformation("Consumer is stopping.");
        }
        catch (Exception ex)
        {
            _logger.LogCritical(ex, "Failed to connect to RabbitMQ");
        }
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        if (_channel != null)
        {
            await _channel.CloseAsync(cancellationToken);
            await _channel.DisposeAsync();
        }
        
        if (_connection != null)
        {
            await _connection.CloseAsync(cancellationToken);
            await _connection.DisposeAsync();
        }
        
        await base.StopAsync(cancellationToken);
    }
}
