using MediatR;
using Nexora.Application.Interfaces;

namespace Nexora.Application.Features.Finance.Transfer;

public class TransferCommand : IRequest<Result>
{
    public int FromUserId { get; set; }
    public string ReceiverLogin { get; set; }
    public decimal Amount { get; set; }
    public string Currency { get; set; }

    public TransferCommand(int fromUserId, string receiverLogin, decimal amount, string currency)
    {
        FromUserId = fromUserId;
        ReceiverLogin = receiverLogin;
        Amount = amount;
        Currency = currency;
    }
}

