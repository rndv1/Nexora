using MediatR;
using Nexora.Application.Interfaces;
using Nexora.Application.DTOs.Finance;

namespace Nexora.Application.Features.Finance.GetTransactionHistory;

public class GetTransactionHistoryQuery : IRequest<Result<List<TransactionHistoryDto>>>
{
    public int UserId { get; set; }
    public DateTime? DateFrom { get; set; }
    public DateTime? DateTo { get; set; }
    public int Skip { get; set; }
    public int Take { get; set; }

    public GetTransactionHistoryQuery(int userId, DateTime? dateFrom, DateTime? dateTo, int skip, int take)
    {
        UserId = userId;
        DateFrom = dateFrom;
        DateTo = dateTo;
        Skip = skip;
        Take = take;
    }
}

