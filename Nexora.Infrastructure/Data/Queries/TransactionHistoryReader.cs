using Nexora.Application;
using Nexora.Application.DTOs.Finance;
using Microsoft.EntityFrameworkCore;
using Nexora.Infrastructure.Database;
using Nexora.Application.Interfaces;

namespace Nexora.Infrastructure.Data.Queries;

public class TransactionHistoryReader : ITransactionHistoryReader
{
    private readonly ApplicationDbContext _dbContext;

    public TransactionHistoryReader(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<TransactionHistoryDto>>> GetHistoryAsync(
        int userId,
        DateTime? dateFrom,
        DateTime? dateTo,
        int skip,
        int take,
        CancellationToken cancellationToken)
    {
        var accountIds = await _dbContext.Accounts
            .Where(a => a.UserId == userId)
            .Select(a => a.Id)
            .ToListAsync(cancellationToken);

        if (accountIds.Count == 0)
        {
            return Result<List<TransactionHistoryDto>>.Failure("Account not found");
        }

        var query = _dbContext.Transactions
            .Where(t => accountIds.Contains(t.SenderAccountId) || accountIds.Contains(t.ReceiverAccountId));

        if (dateFrom.HasValue)
        {
            query = query.Where(t => t.CreatedAt >= dateFrom.Value);
        }

        if (dateTo.HasValue)
        {
            query = query.Where(t => t.CreatedAt <= dateTo.Value);
        }

        var result = await query
            .OrderByDescending(t => t.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(t => new TransactionHistoryDto
            {
                Amount = t.Amount,
                Date = t.CreatedAt,
                Currency = t.Currency,
                SenderName = t.SenderAccount!.User!.Name,
                ReceiverName = t.ReceiverAccount!.User!.Name
            })
            .ToListAsync(cancellationToken);

        return result;
    }
}
