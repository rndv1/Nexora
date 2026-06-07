using Microsoft.AspNetCore.Mvc;
using Nexora.DTOs;
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
            var result = await _userService.RegisterAsync(request.Login, request.Name,
                request.PasswordHash);
            if (result)
            {
                return Ok();
            }
            return BadRequest(new { Message = "User registration failed" });
        }
    }
}
