using System.Text.Json;
using Nexora.Application.Interfaces;
using RabbitMQ.Client;

namespace Nexora.Infrastructure.RabbitMQ;

public class RabbitMqMessageProducer : IMessageProducer
{
    private readonly IConnection _connection;

    public RabbitMqMessageProducer(IConnection connection)
    {
        _connection = connection;
    }

    public async Task SendMessageAsync<T>(T message, CancellationToken token = default)
    {
        await using var channel = await _connection.CreateChannelAsync(cancellationToken: token);

        var queueName = typeof(T).Name;

        await channel.QueueDeclareAsync(
            queue: queueName,
            durable: true,
            exclusive: false,
            autoDelete: false,
            arguments: null,
            cancellationToken: token);

        var body = JsonSerializer.SerializeToUtf8Bytes(message);

        await channel.BasicPublishAsync(
            exchange: string.Empty,
            routingKey: queueName,
            body: body,
            cancellationToken: token);
    }
}