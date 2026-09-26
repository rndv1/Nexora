using Nexora.Domain.Models;

namespace Nexora.Application.Interfaces;

public interface IUserRepository
{
    Task<User?> GetUserByLoginAsync(string login, CancellationToken cancellationToken);
    Task AddUserAsync(User user, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
}
