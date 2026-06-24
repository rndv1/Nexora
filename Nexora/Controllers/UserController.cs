using Microsoft.AspNetCore.Mvc;
using Nexora.DTOs;
using Nexora.DTOs.User;
using Nexora.Services;


namespace Nexora.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IUserService _userService;

        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")] // POST api/user/register
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.RegisterAsync(request.Login, request.Name,
                request.PasswordHash);
            if (result)
            {
                return Ok();
            }
            return BadRequest(new { Message = result.ErrorMessage });
        }

        [HttpPost("login")]
        public async Task<IActionResult> LoginAsync([FromBody] LoginRequest request)
        {
            if (ModelState.IsValid == false)
            {
                return BadRequest(ModelState);
            }

            var result = await _userService.LoginAsync(request.Login, request.PasswordHash);
            if (result)
            {
                return Ok(new { Token = result.Value });
            }
            return Unauthorized(new { Message = result.ErrorMessage });
        }
    }
}
