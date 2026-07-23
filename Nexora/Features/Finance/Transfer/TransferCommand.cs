using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Database;
using Nexora.Models;

namespace Nexora.Features.Finance.Transfer;

public class TransferCommand : IRequest<Result>
{
    public int FromUserId { get; set; }
    public string ReceiverLogin { get; set; }
    public decimal Amount { get; set; }

    public TransferCommand(int fromUserId, string receiverLogin, decimal amount)
    {
        FromUserId = fromUserId;
        ReceiverLogin = receiverLogin;
        Amount = amount;
    }
}

public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;

    public TransferCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
        {
            return Result.Failure("Transfer amount must be greater than 0");
        }

        if (string.IsNullOrWhiteSpace(request.ReceiverLogin))
        {
            return Result.Failure("Receiver login is required");
        }

        var fromUser = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Id == request.FromUserId,
            cancellationToken);
        if (fromUser == null)
        {
            return Result.Failure("User not found");
        }

        var fromAccount = await _dbContext.Accounts.FirstOrDefaultAsync(
            a => a.UserId == fromUser.Id,
            cancellationToken);
        if (fromAccount == null)
        {
            return Result.Failure("Account not found");
        }

        var toUser = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Login == request.ReceiverLogin,
            cancellationToken);
        if (toUser == null)
        {
            return Result.Failure("Recipient not found");
        }

        var toAccount = await _dbContext.Accounts.FirstOrDefaultAsync(
            a => a.UserId == toUser.Id,
            cancellationToken);
        if (toAccount == null)
        {
            return Result.Failure("Account not found");
        }

        if (fromAccount.Balance < request.Amount)
        {
            return Result.Failure("Insufficient funds");
        }

        fromAccount.Balance -= request.Amount;
        toAccount.Balance += request.Amount;

        var transaction = new Transaction
        {
            ReceiverAccountId = toAccount.Id,
            SenderAccountId = fromAccount.Id,
            Amount = request.Amount,
            CreatedAt = DateTime.UtcNow
        };
        _dbContext.Transactions.Add(transaction);

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
