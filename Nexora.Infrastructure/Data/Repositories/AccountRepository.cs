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

    public async Task AddTransactionAsync(Transaction transaction, CancellationToken cancellationToken)
    {
        await _dbContext.Transactions.AddAsync(transaction, cancellationToken);
    }

    public async Task SaveChangesAsync(CancellationToken cancellationToken)
    {
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}