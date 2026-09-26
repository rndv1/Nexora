using Nexora.Domain.Models;

namespace Nexora.Application.Interfaces;

public interface IAccountRepository
{
    Task<Account?> GetAccountByUserIdAsync(int userId, CancellationToken cancellationToken);
    Task<Account?> GetAccountByUserIdAndCurrencyAsync(int userId, string currency, CancellationToken cancellationToken);
    Task<Account?> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken);
    Task IncrementBalanceAsync(int accountId, decimal amount, CancellationToken cancellationToken);
    Task<bool> TryDecrementBalanceAsync(int accountId, decimal amount, CancellationToken cancellationToken);
    Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken);
    Task SaveChangesAsync(CancellationToken cancellationToken);
    Task BeginTransactionAsync(CancellationToken cancellationToken);
    Task CommitTransactionAsync(CancellationToken cancellationToken);
    Task RollbackTransactionAsync(CancellationToken cancellationToken);
}
