using Microsoft.EntityFrameworkCore;
using Nexora.Database;
using Nexora.DTOs.Finance;
using Nexora.Models;

namespace Nexora.Services;

public class FinanceService : IFinanceService
{
    private readonly ApplicationDbContext _dbContext;

    public FinanceService(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<decimal>> GetBalanceAsync(int userId)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(predicate: u => u.Id == userId);
        if (user == null)
        {
            return Result<decimal>.Failure("User not found");
        }
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(predicate: a => a.UserId == user.Id);
        if (account == null)
        {
            return Result<decimal>.Failure("Account not found");
        }
        return Result<decimal>.Success(account.Balance);
    }

    public async Task<Result> DepositAsync(int userId, decimal amount)
    {
        if (amount <= 0)
        {
            return Result.Failure("Deposit amount must be greater than 0");
        }
        var user = await _dbContext.Users.FirstOrDefaultAsync(predicate: u => u.Id == userId);
        if (user == null)
        {
            return Result.Failure("User not found");
        }
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(predicate: a => a.UserId == user.Id);
        if (account == null)
        {
            return Result.Failure("Account not found");
        }

        account.Balance += amount;

        await _dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> TransferAsync(int fromUserId, string receiverLogin, decimal amount)
    {
        if (amount <= 0)
        {
            return Result.Failure("Transfer amount must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(receiverLogin))
        {
            return Result.Failure("Receiver login is required");
        }

        var fromUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == fromUserId);

        if (fromUser == null)
        {
            return Result.Failure("User not found");
        }
        var fromAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == fromUser.Id);

        if (fromAccount == null)
        {
            return Result.Failure("Account not found");
        }
        var toUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == receiverLogin);
        if (toUser == null)
        {
            return Result.Failure("Recipient not found");
        }
        var toAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == toUser.Id);
        if (toAccount == null)
        {
            return Result.Failure("Account not found");
        }
        if (fromAccount.Balance < amount)
        {
            return Result.Failure("Insufficient funds");
        }
        fromAccount.Balance -= amount;
        toAccount.Balance += amount;

        var transaction = new Transaction
        {
            ReceiverAccountId = toAccount.Id,
            SenderAccountId = fromAccount.Id,
            Amount = amount,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Transactions.Add(transaction);

        await _dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result<List<TransactionHistoryResponse>>> GetTransactionHistoryAsync(int userId,
        DateTime? dateFrom, DateTime? dateTo, int skip, int take)
    {
        if (skip < 0)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("Offset cannot be negative");
        }

        if (take is < 1 or > 100)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("Limit must be between 1 and 100");
        }

        if (dateFrom.HasValue && dateTo.HasValue && dateFrom.Value > dateTo.Value)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("From date must not be later than To date");
        }

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == userId);

        if (account == null)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("Account not found");
        }

        var transactions = _dbContext.Transactions.Where(x => x.SenderAccountId
            == account.Id || x.ReceiverAccountId == account.Id);

        if (dateFrom != null)
        {
            transactions = transactions.Where(x => x.CreatedAt >= dateFrom);
        }

        if (dateTo != null)
        {
            transactions = transactions.Where(x => x.CreatedAt <= dateTo);
        }
        transactions = transactions.OrderBy(x => x.CreatedAt).Skip(skip).Take(take);

        var dbTransactions = await transactions.ToListAsync();

        var result = new List<TransactionHistoryResponse>();
        var allSender = dbTransactions.Select(x => x.SenderAccountId).Distinct().ToList();
        var allReceiver = dbTransactions.Select(x => x.ReceiverAccountId).Distinct().ToList();
        var allAccount = allSender.ToHashSet();

        foreach (var receiver in allReceiver)
        {
            allAccount.Add(receiver);
        }

        var names = await _dbContext.Accounts.Where(x => allAccount.Contains(x.Id)).Join(_dbContext.Users,
            acc => acc.UserId,
            u => u.Id, (acc, u) => new
            {
                Name = u.Name,
                AccId = acc.Id
            }).ToDictionaryAsync(x => x.AccId);

        foreach (var transaction in dbTransactions)
        {
            var senderName = names[transaction.SenderAccountId].Name;
            var receiverName = names[transaction.ReceiverAccountId].Name;
            result.Add(new TransactionHistoryResponse
            {
                SenderName = senderName,
                ReceiverName = receiverName,
                Amount = transaction.Amount,
                Date = transaction.CreatedAt,
            });
        }

        return result;
    }
}
