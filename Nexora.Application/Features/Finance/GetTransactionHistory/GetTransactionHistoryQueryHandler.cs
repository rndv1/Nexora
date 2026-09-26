using MediatR;
using Nexora.Application.Interfaces;
using Nexora.Application.DTOs.Finance;

namespace Nexora.Application.Features.Finance.GetTransactionHistory;

public class GetTransactionHistoryQueryHandler
    : IRequestHandler<GetTransactionHistoryQuery, Result<List<TransactionHistoryDto>>>
{
    private readonly ITransactionHistoryReader _reader;

    public GetTransactionHistoryQueryHandler(ITransactionHistoryReader reader)
    {
        _reader = reader;
    }

    public async Task<Result<List<TransactionHistoryDto>>> Handle(
        GetTransactionHistoryQuery request,
        CancellationToken cancellationToken)
    {
        var result = await _reader.GetHistoryAsync(
            request.UserId,
            request.DateFrom,
            request.DateTo,
            request.Skip,
            request.Take,
            cancellationToken);

        return result;
    }
}



