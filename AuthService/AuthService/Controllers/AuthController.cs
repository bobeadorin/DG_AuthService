using AuthService.Constant;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpGet("/")]
        public IActionResult Get() { 

            return Ok("it works");
        }

        [HttpGet("/users")]
        public async Task<IActionResult> GetAllUsers()
        {

            return Ok(await _userService.GetAllUsers());
        }


        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] User user)
        {
            var registrationResponse = await _userService.AddUser(user);

            return Ok(registrationResponse);
        }

    }
}
