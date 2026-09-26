using MediatR;
using Nexora.Application.Interfaces;

namespace Nexora.Application.Features.Finance.Transfer;

public class TransferCommandHandler : IRequestHandler<TransferCommand, Result>
{
    private readonly IAccountRepository _accountRepository;
    private readonly IUserRepository _userRepository;

    public TransferCommandHandler(IAccountRepository accountRepository, IUserRepository userRepository)
    {
        _accountRepository = accountRepository;
        _userRepository = userRepository;
    }

    public async Task<Result> Handle(TransferCommand request, CancellationToken cancellationToken)
    {
        if (request.Amount <= 0)
        {
            return Result.Failure("Transfer amount must be greater than 0");
        }

        var sourceAccount = await _accountRepository.GetAccountByUserIdAndCurrencyAsync(request.FromUserId, request.Currency, cancellationToken);
        if (sourceAccount == null)
        {
            return Result.Failure("Source account not found");
        }

        if (sourceAccount.Balance < request.Amount)
        {
            return Result.Failure("Insufficient funds");
        }

        var receiverUser = await _userRepository.GetUserByLoginAsync(request.ReceiverLogin, cancellationToken);
        if (receiverUser == null)
        {
            return Result.Failure("Receiver not found");
        }

        var destAccount = await _accountRepository.GetAccountByUserIdAndCurrencyAsync(receiverUser.Id, request.Currency, cancellationToken);
        if (destAccount == null)
        {
            return Result.Failure("Destination account not found");
        }

        if (sourceAccount.Id == destAccount.Id)
        {
            return Result.Failure("Cannot transfer to the same account");
        }

        sourceAccount.Balance -= request.Amount;
        destAccount.Balance += request.Amount;

        await _accountRepository.SaveChangesAsync(cancellationToken);

        return Result.Success();
    }
}


