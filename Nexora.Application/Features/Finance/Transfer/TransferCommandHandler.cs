using System.Text.Json;
using MediatR;
using Nexora.Application.Events;
using Nexora.Application.Interfaces;
using Nexora.Domain.Models;

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

        try
        {
            await _accountRepository.BeginTransactionAsync(cancellationToken);

            var sourceAccount = await _accountRepository.GetAccountByUserIdAndCurrencyAsync(request.FromUserId, request.Currency, cancellationToken);
            if (sourceAccount == null)
            {
                await _accountRepository.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Source account not found");
            }

            var receiverUser = await _userRepository.GetUserByLoginAsync(request.ReceiverLogin, cancellationToken);
            if (receiverUser == null)
            {
                await _accountRepository.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Receiver not found");
            }

            var destAccount = await _accountRepository.GetAccountByUserIdAndCurrencyAsync(receiverUser.Id, request.Currency, cancellationToken);
            if (destAccount == null)
            {
                await _accountRepository.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Destination account not found");
            }

            if (sourceAccount.Id == destAccount.Id)
            {
                await _accountRepository.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Cannot transfer to the same account");
            }

            bool decremented = await _accountRepository.TryDecrementBalanceAsync(sourceAccount.Id, request.Amount, cancellationToken);
            if (!decremented)
            {
                await _accountRepository.RollbackTransactionAsync(cancellationToken);
                return Result.Failure("Insufficient funds");
            }

            await _accountRepository.IncrementBalanceAsync(destAccount.Id, request.Amount, cancellationToken);

            var transaction = new Nexora.Domain.Models.Transaction
            {
                SenderAccountId = sourceAccount.Id,
                ReceiverAccountId = destAccount.Id,
                Amount = request.Amount,
                Currency = request.Currency,
                CreatedAt = DateTime.UtcNow
            };
            await _accountRepository.AddTransactionAsync(transaction, cancellationToken);

            var eventPayload = JsonSerializer.Serialize(new TransactionCreatedEvent
            {
                SenderId = request.FromUserId,
                ReceiverId = receiverUser.Id,
                Amount = transaction.Amount,
                Currency = transaction.Currency
            });

            var outboxMessage = new OutboxMessage
            {
                Id = Guid.NewGuid(),
                Type = nameof(TransactionCreatedEvent),
                Payload = eventPayload,
                CreatedAt = DateTime.UtcNow
            };
            await _accountRepository.AddOutboxMessageAsync(outboxMessage, cancellationToken);

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


