using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Nexora.API.DTOs.User;
using Nexora.Application.Features.User.UserLogin;
using Nexora.Application.Features.User.UserRegister;


namespace Nexora.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IMediator _mediator;

        public UserController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request, [FromServices] IValidator<RegisterRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new UserRegisterCommand(request.Login!, request.Name!, request.PasswordHash!);
            var result = await _mediator.Send(command);
            if (result)
            {
                return Ok();
            }
            return BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request, [FromServices] IValidator<LoginRequest> validator)
        {
            var validationResult = await validator.ValidateAsync(request);
            if (!validationResult.IsValid)
            {
                return BadRequest(validationResult.ToDictionary());
            }

            var command = new UserLoginCommand(request.Login!, request.PasswordHash!);
            var result = await _mediator.Send(command);

            if (result)
            {
                return Ok(new { Token = result.Value });
            }
            return Unauthorized(new { Message = result.ErrorMessage });
        }
    }
}
