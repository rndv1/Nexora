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

        try
        {
            await _accountRepository.BeginTransactionAsync(cancellationToken);

            var account = await _accountRepository.GetAccountByUserIdAndCurrencyAsync(request.UserId, request.Currency, cancellationToken);
            if (account == null)
            {
                await _accountRepository.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Account not found");
            }

            await _accountRepository.IncrementBalanceAsync(account.Id, request.Amount, cancellationToken);

            var transaction = new Nexora.Domain.Models.Transaction
            {
                ReceiverAccountId = account.Id,
                SenderAccountId = account.Id, // Self-deposit
                Amount = request.Amount,
                Currency = request.Currency,
                CreatedAt = DateTime.UtcNow
            };
            await _accountRepository.AddTransactionAsync(transaction, cancellationToken);

            await _accountRepository.SaveChangesAsync(cancellationToken);
            await _accountRepository.CommitTransactionAsync(cancellationToken);

            return Result.Success();
        }
        catch (Exception)
        {
            await _accountRepository.RollbackTransactionAsync(cancellationToken);
            throw;
        }
    }
}


