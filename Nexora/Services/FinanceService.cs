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

    public async Task<Result<decimal>> GetBalanceAsync(string token)
    {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
        if (session == null)
        {
            return Result<decimal>.Failure("Пользователь не авторизован");
        }
        var user = await _dbContext.Users.FirstOrDefaultAsync(predicate: u => u.Id == session.UserId);
        if (user == null)
        {
            return Result<decimal>.Failure("Пользователь не найден");
        }
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(predicate: a => a.UserId == user.Id);
        if (account == null)
        {
            return Result<decimal>.Failure("Аккаунт не найден");
        }
        return Result <decimal>.Success(account.Balance);
    }

    public async Task<Result> DepositAsync(string token, decimal amount)
    {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
        if (session == null)
        {
            return Result<decimal>.Failure("Пользователь не авторизован");
        }
        var user = await _dbContext.Users.FirstOrDefaultAsync(predicate: u => u.Id == session.UserId);
        if (user == null)
        {
            return Result<decimal>.Failure("Пользователь не найден");
        }
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(predicate: a => a.UserId == user.Id);
        if (account == null)
        {
            return Result<decimal>.Failure("Аккаунт не найден");
        }

        account.Balance += amount;
        
        await _dbContext.SaveChangesAsync();
        return Result.Success();
    }

    public async Task<Result> TransferAsync(string fromToken, string receiverLogin, decimal amount)
    {
        var fromSession = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == fromToken);
        if (fromSession == null)
        {
            return Result.Failure("Пользователь не авторизован");
        }
        
        var fromUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Id == fromSession.UserId);

        if (fromUser == null)
        {
            return Result.Failure("Пользователь не найден");
        }
        var fromAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == fromUser.Id);

        if (fromAccount == null)
        {
            return Result.Failure("Аккаунт не найден"); 
        }
        var toUser = await _dbContext.Users.FirstOrDefaultAsync(u => u.Login == receiverLogin);
        if (toUser == null)
        {
            return Result.Failure("Получатель не найден");
        }
        var toAccount = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == toUser.Id);
        if (toAccount == null)
        {
            return Result.Failure("Аккаунт не найден"); 
        }
        if (fromAccount.Balance < amount)
        {
            return Result.Failure("Недостаточно средств");
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

    public async Task<Result<List<TransactionHistoryResponse>>> GetTransactionHistoryAsync(string token,
        DateTime? dateFrom, DateTime? dateTo, int skip , int take)
    {
        var session = await _dbContext.Sessions.FirstOrDefaultAsync(s => s.Token == token);
        if (session == null)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("Пользователь не авторизован");
        }
        
        var account = await _dbContext.Accounts.FirstOrDefaultAsync(a => a.UserId == session.UserId);

        if (account == null)
        {
            return Result<List<TransactionHistoryResponse>>.Failure("Аккаунт не найден");
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