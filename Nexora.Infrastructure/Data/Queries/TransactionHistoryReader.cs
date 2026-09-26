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

        var query = _dbContext.Transactions.AsQueryable();

        query = query.Where(x => accountIds.Contains(x.SenderAccountId) || accountIds.Contains(x.ReceiverAccountId));

        if (dateFrom.HasValue)
        {
            query = query.Where(x => x.CreatedAt >= dateFrom.Value);
        }
        if (dateTo.HasValue)
        {
            query = query.Where(x => x.CreatedAt <= dateTo.Value);
        }

        var projectedQuery = query
            .OrderBy(x => x.CreatedAt)
            .Skip(skip)
            .Take(take)
            .Select(t => new TransactionHistoryDto
            {
                Amount = t.Amount,
                Date = t.CreatedAt,
                Currency = t.Currency,
                SenderName = _dbContext.Accounts
                    .Where(a => a.Id == t.SenderAccountId)
                    .Select(a => a.User!.Name)
                    .FirstOrDefault() ?? "Unknown",
                ReceiverName = _dbContext.Accounts
                    .Where(a => a.Id == t.ReceiverAccountId)
                    .Select(a => a.User!.Name)
                    .FirstOrDefault() ?? "Unknown"
            });

        var result = await projectedQuery.ToListAsync(cancellationToken);

        return result;
    }
}
