using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Database;

namespace Nexora.Features.Finance.Deposit;

public class DepositCommand : IRequest<Result>
{
    public int UserId { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }

    public DepositCommand(int userId, decimal amount, string currency)
    {
        UserId = userId;
        Amount = amount;
        Currency = currency;
    }
}

public class DepositCommandHandler : IRequestHandler<DepositCommand, Result>
{
    private readonly ApplicationDbContext _dbContext;

    public DepositCommandHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
        {
            return Result.Failure("Deposit amount must be greater than 0");
        }

        var user = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            cancellationToken);
        if (user == null)
        {
            return Result.Failure("User not found");
        }

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(
            a => a.UserId == user.Id && a.Currency == request.Currency,
            cancellationToken);
        if (account == null)
        {
            return Result.Failure("Account not found");
        }

        account.Balance += request.Amount;

        await _dbContext.SaveChangesAsync(cancellationToken);
        return Result.Success();
    }
}
