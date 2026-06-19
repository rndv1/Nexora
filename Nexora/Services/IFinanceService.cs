using Nexora.DTOs.Finance;

namespace Nexora.Services
{
public interface IFinanceService
{
    Task <Result<decimal>> GetBalanceAsync(int userId);
    Task<Result> DepositAsync(int userId, decimal amount);
    Task<Result> TransferAsync(int fromUserId, string receiverLogin, decimal amount);

    Task<Result<List<TransactionHistoryResponse>>> GetTransactionHistoryAsync(int userId,
        DateTime? dateFrom, DateTime? dateTo, int skip, int take);
}
}