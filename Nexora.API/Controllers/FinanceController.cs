using AutoMapper;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.Application.DTOs.Finance;
using Nexora.API.DTOs.Finance;
using Nexora.API.Attributes;
using Nexora.Application.Features.Finance.Deposit;
using Nexora.Application.Features.Finance.GetBalance;
using Nexora.Application.Features.Finance.GetTransactionHistory;
using Nexora.Application.Features.Finance.Transfer;

namespace Nexora.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [MyAuthorize]
    public class FinanceController : Controller
    {
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public FinanceController(IMediator mediator, IMapper mapper)
        {
            _mediator = mediator;
            _mapper = mapper;
        }

        [HttpGet("balance")]
        public async Task<IActionResult> GetBalanceAsync(
            [FromQuery] BalanceRequest request,
            [FromServices] IValidator<BalanceRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var balanceResult = await _mediator.Send(new GetBalanceQuery(GetUserId(), request.Currency!));
            if (balanceResult.IsSuccess)
            {
                return Ok(new BalanceResponse
                {
                    Balance = balanceResult.Value,
                    Currency = request.Currency!
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

            var depositResult = await _mediator.Send(new DepositCommand(GetUserId(), request.Amount, request.Currency!));
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
                new TransferCommand(GetUserId(), request.ReceiverLogin!, request.Amount, request.Currency!));
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
                var response = _mapper.Map<List<TransactionHistoryResponse>>(historyResult.Value);
                return Ok(response);
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
