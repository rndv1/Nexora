using Microsoft.EntityFrameworkCore;
using Nexora.Application.Interfaces;
using Nexora.Domain.Models;
using Nexora.Infrastructure.Database;

namespace Nexora.Infrastructure.Data.Repositories;

public class AccountRepository : IAccountRepository
{
    private readonly ApplicationDbContext _dbContext;

    public AccountRepository(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Account?> GetAccountByUserIdAsync(int userId, CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId, cancellationToken);
    }

    public async Task<Account?> GetAccountByUserIdAndCurrencyAsync(int userId, string currency, CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId && a.Currency == currency, cancellationToken);
    }

    public async Task<Account?> GetAccountByIdAsync(int accountId, CancellationToken cancellationToken)
    {
        return await _dbContext.Accounts.FirstOrDefaultAsync(a => a.Id == accountId, cancellationToken);
    }

    public async Task IncrementBalanceAsync(int accountId, decimal amount, CancellationToken cancellationToken)
    {
        await _dbContext.Accounts
            .Where(a => a.Id == accountId)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Balance, a => a.Balance + amount), cancellationToken);
    }

    public async Task<bool> TryDecrementBalanceAsync(int accountId, decimal amount, CancellationToken cancellationToken)
    {
        int rows = await _dbContext.Accounts
            .Where(a => a.Id == accountId && a.Balance >= amount)
            .ExecuteUpdateAsync(s => s.SetProperty(a => a.Balance, a => a.Balance - amount), cancellationToken);
        
        return rows > 0;
    }

    public async Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
    }

    public async Task AddOutboxMessageAsync(OutboxMessage message, CancellationToken cancellationToken)
    {
        await _dbContext.OutboxMessages.AddAsync(message, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task BeginTransactionAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.BeginTransactionAsync(System.Data.IsolationLevel.Serializable, cancellationToken);
    }

    public async Task CommitTransactionAsync(CancellationToken cancellationToken)
    {
        await _dbContext.Database.CommitTransactionAsync(cancellationToken);
    }

    public async Task RollbackTransactionAsync(CancellationToken cancellationToken)
    {
        if (_dbContext.Database.CurrentTransaction != null)
        {
            try
            {
                await _dbContext.Database.RollbackTransactionAsync(CancellationToken.None);
            }
            catch { }
        }
    }
}