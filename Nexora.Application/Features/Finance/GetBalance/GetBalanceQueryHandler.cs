using MediatR;
using Nexora.Application.Interfaces;


namespace Nexora.Application.Features.Finance.GetBalance;

public class GetBalanceQueryHandler : IRequestHandler<GetBalanceQuery, Result<decimal>>
{
    private readonly IAccountRepository _accountRepository;

    public GetBalanceQueryHandler(IAccountRepository accountRepository)
    {
        _accountRepository = accountRepository;
    }

    public async Task<Result<decimal>> Handle(GetBalanceQuery request, CancellationToken cancellationToken)
    {
        var account = await _accountRepository.GetAccountByUserIdAndCurrencyAsync(request.UserId, request.Currency, cancellationToken);

        if (account == null)
        {
            return Result<decimal>.Failure("Account not found");
        }

        return Result<decimal>.Success(account.Balance);
    }
}



