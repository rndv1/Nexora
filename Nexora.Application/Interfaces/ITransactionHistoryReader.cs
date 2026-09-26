namespace Nexora.Application.Interfaces;

using Nexora.Application.DTOs.Finance;

public interface ITransactionHistoryReader
{
    Task<Result<List<TransactionHistoryDto>>> GetHistoryAsync(
        int userId,
        DateTime? dateFrom,
        DateTime? dateTo,
        int skip,
        int take,
        CancellationToken cancellationToken);
}
