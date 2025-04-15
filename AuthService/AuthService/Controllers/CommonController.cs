using System.IdentityModel.Tokens.Jwt;
using AuthService.Filters;
using AuthService.Helpers.HelpersInterfaces;
using AuthService.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthService.Controllers
{
    [ApiController]
    public class CommonController:ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ITokenGenerator _tokenGenerator;
        private readonly ILogger<CommonController> _logger;
        private readonly IConfiguration _config;

        public CommonController( IUserService userService, ILogger<CommonController> logger, IConfiguration config, ITokenGenerator tokenGenerator)
        {
            _userService = userService;
            _logger = logger;
            _config = config;
            _tokenGenerator = tokenGenerator;
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


        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUser()
        {
            // Best practice: Get user ID from pre-validated claims
            var userId = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("User ID claim missing");
            }

            var (userData, isUser) = await _userService.GetUserById(new Guid(userId));

            if (!isUser || userData is null) return NotFound("The user was not found");
            

            return Ok(userData);
        }
    }
}
