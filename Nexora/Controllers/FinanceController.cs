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
        public async Task<IActionResult> DepositAsync([FromBody] DepositRequest request)
        {
            var depositResult = await _financeService.DepositAsync(request.Token, request.Amount);
            if (depositResult.IsSuccess)
            {
                return Ok();
            }
            return BadRequest(new { Message = depositResult.ErrorMessage });
        }
    }
}