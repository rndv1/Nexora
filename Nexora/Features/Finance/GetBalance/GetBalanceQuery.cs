using MediatR;
using Microsoft.EntityFrameworkCore;
using Nexora.Database;

namespace Nexora.Features.Finance.GetBalance;

public class GetBalanceQuery : IRequest<Result<decimal>>
{
    public int UserId { get; set; }
    public string Currency { get; set; }

    public GetBalanceQuery(int userId, string currency)
    {
        UserId = userId;
        Currency = currency;
    }
}

public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, Result<decimal>>
{
    private readonly ApplicationDbContext _dbContext;

    public GetBalanceQueryHandler(ApplicationDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Result<decimal>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var user = await _dbContext.Users.FirstOrDefaultAsync(
            u => u.Id == request.UserId,
            cancellationToken);
        if (user == null)
        {
            return Result<decimal>.Failure("User not found");
        }

        var account = await _dbContext.Accounts.FirstOrDefaultAsync(
            a => a.UserId == user.Id && a.Currency == request.Currency,
            cancellationToken);
        if (account == null)
        {
            return Result<decimal>.Failure("Account not found");
        }

        return Result<decimal>.Success(account.Balance);
    }
}
