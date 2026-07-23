using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Database;
using Nexora.DTOs.Finance;

namespace Nexora.Features.Finance.GetTransactionHistory;

public class GetTransactionHistoryQuery : IRequest<Result<List<TransactionHistoryResponse>>>
{
    public int UserId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }

    public GetTransactionHistoryQuery(int userId, DateTime? dateFrom, DateTime? dateTo, int skip, int take)
    {
        UserId = userId;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Skip = skip;
        Take = take;
    }
}

public class GetTransactionHistoryQueryHandler
    : IRequestHandler<GetTransactionHistoryQuery, Result<List<TransactionHistoryResponse>>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetTransactionHistoryQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<List<TransactionHistoryResponse>>> Handle(
        GetTransactionHistoryQuery request,
        CancellationToken cancellationToken)
    {
        if (request.Skip < 0)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("Offset cannot be negative");
        }

        if (request.Take is < 1 or > 100)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("Limit must be between 1 and 100");
        }

        if (request.DateFrom.HasValue && request.DateTo.HasValue && request.DateFrom.Value > request.DateTo.Value)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("From date must not be later than To date");
        }

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(
            a => a.UserId == request.UserId,
            cancellationToken);
        if (account == null)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("Account not found");
        }

        var transactions = _dbContext.Transactions.Where(
            x => x.SenderAccountId == account.Id || x.ReceiverAccountId == account.Id);

        if (request.DateFrom != null)
        {
            transactions = transactions.Where(x => x.CreatedAt >= request.DateFrom);
        }

        if (request.DateTo != null)
        {
            transactions = transactions.Where(x => x.CreatedAt <= request.DateTo);
        }

        transactions = transactions.OrderBy(x => x.CreatedAt).Skip(request.Skip).Take(request.Take);

        var dbTransactions = await transactions.ToListAsync(cancellationToken);

        var result = new List<TransactionHistoryResponse>();
        var allSender = dbTransactions.Select(x => x.SenderAccountId).Distinct().ToList();
        var allReceiver = dbTransactions.Select(x => x.ReceiverAccountId).Distinct().ToList();
        var allAccount = allSender.ToHashSet();

        foreach (var receiver in allReceiver)
        {
            allAccount.Add(receiver);
        }

        var names = await _dbContext.Accounts.Where(x => allAccount.Contains(x.Id)).Join(
            _dbContext.Users,
            acc => acc.UserId,
            u => u.Id,
            (acc, u) => new
            {
                Name = u.Name,
                AccId = acc.Id
            }).ToDictionaryAsync(x => x.AccId, cancellationToken);

        foreach (var transaction in dbTransactions)
        {
            var senderName = names[transaction.SenderAccountId].Name;
            var receiverName = names[transaction.ReceiverAccountId].Name;
            result.Add(new TransactionHistoryResponse
            {
                SenderName = senderName,
                ReceiverName = receiverName,
                Amount = transaction.Amount,
                Date = transaction.CreatedAt
            });
        }

        return result;
    }
}
