using Nexora.Domain.Models;
using Microsoft.EntityFrameworkCore;
using Nexora.Application.Interfaces;
using Nexora.Infrastructure.Database;


namespace Nexora.Infrastructure.Data.Repositories;

public class UserRepository : IUserRepository
{
    private readonly ApplicationDbContext _dbContext;

    public UserRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken)
    {
        return await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == login, cancellationToken);
    }

    public async Task AddUserAsync(User user, CancellationToken cancellationToken)
    {
        await _dbContext.Users.AddAsync(user, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
