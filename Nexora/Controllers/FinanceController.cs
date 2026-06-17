using Microsoft.AspNetCore.Mvc;
using Nexora.DTOs.Finance;
using Nexora.Services;

namespace Nexora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class FinanceController : Controller
    {
        private readonly IFinanceService _financeService;
        
        public FinanceController(IFinanceService financeService)
        {
            _financeService = financeService;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync([FromHeader]string token)
        {
            var balanceResult = await _financeService.GetBalanceAsync(token);
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
        public async Task<IActionResult> DepositAsync([FromHeader]string token, [FromBody] DepositRequest request)
        {
            var depositResult = await _financeService.DepositAsync(token, request.Amount);
            if (depositResult.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(new { Message = depositResult.ErrorMessage });
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferAsync([FromHeader]string token, [FromBody] TransferRequest request)
        {
            var transferResult =
                await _financeService.TransferAsync(token, request.ReceiverLogin, request.Amount);
            if (transferResult.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(new { Message = transferResult.ErrorMessage });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistoryAsync([FromHeader]string token, [FromQuery]TransactionHistoryRequest request)
        {
            var historyResult = await _financeService.GetTransactionHistoryAsync(token, request.From,
                request.To, request.Offset, request.Limit);
            if (historyResult.IsSuccess)
            {
                return Ok(historyResult.Value);
            }
            return BadRequest(new { Message = historyResult.ErrorMessage });
        }
    }
}