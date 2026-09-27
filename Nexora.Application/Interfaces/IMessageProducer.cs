namespace Nexora.Application.Interfaces;

public interface IMessageProducer
{
    Task SendMessageAsync<T>(T message, CancellationToken token = default);
}