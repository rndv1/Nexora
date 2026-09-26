using MediatR;
using Nexora.Application.Interfaces;


namespace Nexora.Application.Features.Finance.GetBalance;

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

