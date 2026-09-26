using Microsoft.EntityFrameworkCore;
using Nexora.Application.Interfaces;
using Nexora.Domain.Models;
using Nexora.Infrastructure.Database;

namespace Nexora.Infrastructure.Data.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _dbContext;

    public SessionRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Session?> GetByTokenAsync(string token, CancellationToken cancellationToken)
    {
        return await _dbContext.Sessions
            .FirstOrDefaultAsync(s => s.Token == token, cancellationToken);
    }

    public async Task UpsertAsync(Session session, CancellationToken cancellationToken)
    {
        var existing = await _dbContext.Sessions
            .FirstOrDefaultAsync(s => s.UserId == session.UserId, cancellationToken);

        if (existing == null)
        {
            await _dbContext.Sessions.AddAsync(session, cancellationToken);
        }
        else
        {
            existing.Token = session.Token;
            existing.ExpiresAt = session.ExpiresAt;
        }
    }

    public async Task DeleteExpiredAsync(CancellationToken cancellationToken)
    {
        var now = DateTime.UtcNow;
        await _dbContext.Sessions
            .Where(s => s.ExpiresAt < now)
            .ExecuteDeleteAsync(cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
