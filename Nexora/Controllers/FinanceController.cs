using Microsoft.AspNetCore.Mvc;
using Nexora.Attributes;
using Nexora.DTOs.Finance;
using Nexora.Services;

namespace Nexora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyAuthorize]
    public class FinanceController : Controller
    {
        private readonly IFinanceService _financeService;

        public FinanceController(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync()
        {
            var balanceResult = await _financeService.GetBalanceAsync(GetUserId());
            if (balanceResult.IsSuccess)
            {
                return Ok(new BalanceResponse
                {
                    Balance = balanceResult.Value
                });
            }
            return BadRequest(new { Message = balanceResult.ErrorMessage });
        }

        [HttpPost("deposit")]
        public async Task<IActionResult> DepositAsync([FromBody] DepositRequest request)
        {
            var depositResult = await _financeService.DepositAsync(
                GetUserId(),
                request.Amount);
            if (depositResult.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(new { Message = depositResult.ErrorMessage });
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferAsync([FromBody] TransferRequest request)
        {
            var transferResult =
                await _financeService.TransferAsync(
                    GetUserId(),
                    request.ReceiverLogin,
                    request.Amount);
            if (transferResult.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(new { Message = transferResult.ErrorMessage });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistoryAsync([FromQuery] TransactionHistoryRequest request)
        {
            var historyResult = await _financeService.GetTransactionHistoryAsync(
                GetUserId(),
                request.From,
                request.To,
                request.Offset,
                request.Limit);
            if (historyResult.IsSuccess)
            {
                return Ok(historyResult.Value);
            }
            return BadRequest(new { Message = historyResult.ErrorMessage });
        }

        internal int GetUserId()
        {
            var userId = HttpContext.Items[Constants.UserIdContextParameterName] as int?;
            return userId!.Value;
        }
    }
}