using Nexora.Database;
using Microsoft.EntityFrameworkCore;

namespace Nexora.Services;

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
                var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

                var now = DateTime.UtcNow;

                var deleteCount = await db.Sessions
                    .Where(x => x.ExpiresAt < now)
                    .ExecuteDeleteAsync(stoppingToken);
                
                _logger.LogInformation(
                    "Deleted {DeleteCount} expired sessions",
                    deleteCount);
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