using Nexora.Domain.Models;

namespace Nexora.Application.Interfaces;

public interface ISessionRepository
{
    Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken);
    Task UpsertAsync(Session session, CancellationToken cancellationToken);
    Task DeleteExpiredAsync(CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
