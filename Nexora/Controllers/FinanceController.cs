using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Attributes;
using Nexora.DTOs.Finance;
using Nexora.Features.Finance.Deposit;
using Nexora.Features.Finance.GetBalance;
using Nexora.Features.Finance.GetTransactionHistory;
using Nexora.Features.Finance.Transfer;

namespace Nexora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyAuthorize]
    public class FinanceController : Controller
    {
        private readonly IMediator _mediator;

        public FinanceController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync()
        {
            var balanceResult = await _mediator.Send(new GetBalanceQuery(GetUserId()));
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
        public async Task<IActionResult> DepositAsync(
            [FromBody] DepositRequest request,
            [FromServices] IValidator<DepositRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var depositResult = await _mediator.Send(new DepositCommand(GetUserId(), request.Amount));
            if (depositResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new { Message = depositResult.ErrorMessage });
        }

        [HttpPost("transfer")]
        public async Task<IActionResult> TransferAsync(
            [FromBody] TransferRequest request,
            [FromServices] IValidator<TransferRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var transferResult = await _mediator.Send(
                new TransferCommand(GetUserId(), request.ReceiverLogin!, request.Amount));
            if (transferResult.IsSuccess)
            {
                return Ok();
            }

            return BadRequest(new { Message = transferResult.ErrorMessage });
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetTransactionHistoryAsync(
            [FromQuery] TransactionHistoryRequest request,
            [FromServices] IValidator<TransactionHistoryRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var historyResult = await _mediator.Send(
                new GetTransactionHistoryQuery(GetUserId(), request.From, request.To, request.Offset, request.Limit));
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
