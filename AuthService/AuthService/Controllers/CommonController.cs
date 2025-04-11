using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    public class CommonController:ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<CommonController> _logger;
        private readonly IConfiguration _config;

        public CommonController( IUserService userService, ILogger<CommonController> logger, IConfiguration config)
        {
            _userService = userService;
            _logger = logger;
            _config = config;
        }

        [HttpGet("/")]
        public IActionResult Get()
        {

            return Ok("it works xd");
        }

        [Authorize]
        [HttpGet("/users")]
        public async Task<IActionResult> GetAllUsers()
        {

            return Ok(await _userService.GetAllUsers());
        }
    }
}
