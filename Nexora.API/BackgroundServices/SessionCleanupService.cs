using Nexora.Application.Interfaces;

namespace Nexora.API.BackgroundServices;

public class SessionCleanupService : BackgroundService
{
    private static readonly TimeSpan Delay = TimeSpan.FromMinutes(10);

    private readonly IServiceScopeFactory _scopeFactory;
    private readonly ILogger<SessionCleanupService> _logger;

    public SessionCleanupService(IServiceScopeFactory scopeFactory, ILogger<SessionCleanupService> logger)
    {
        _scopeFactory = scopeFactory;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();
                var sessionRepository = scope.ServiceProvider.GetRequiredService<ISessionRepository>();

                await sessionRepository.DeleteExpiredAsync(stoppingToken);

                _logger.LogInformation("Expired sessions cleanup completed");
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred executing {Method}", "Cleanup");
            }

            try
            {
                await Task.Delay(Delay, stoppingToken);
            }
            catch (OperationCanceledException)
                when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
        }
    }
}