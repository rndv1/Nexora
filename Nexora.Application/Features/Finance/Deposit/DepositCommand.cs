using MediatR;
using Nexora.Application.Interfaces;

namespace Nexora.Application.Features.Finance.Deposit;

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

