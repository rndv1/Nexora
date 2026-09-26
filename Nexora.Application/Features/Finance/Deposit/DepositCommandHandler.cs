using MediatR;
using Nexora.Application.Interfaces;

namespace Nexora.Application.Features.Finance.Deposit;

public class DepositCommandHandler : IRequestHandler<DepositCommand, Result>
{
    private readonly IAccountRepository _accountRepository;

    public DepositCommandHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result> Handle(DepositCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
        {
            return Result.Failure("Deposit amount must be greater than 0");
        }

        var account = await _accountRepository.GetAccountByUserIdAndCurrencyAsync(request.UserId, request.Currency, cancellationToken);
        if (account == null)
        {
            return Result.Failure("Account not found");
        }

        account.Balance += request.Amount;

        await _accountRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


