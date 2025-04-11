using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using AuthService.Constant;
using AuthService.Helpers.HelpersInterfaces;
using AuthService.Models;
using AuthService.Models.ControllerResponses;
using AuthService.Services.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("[controller]")] 
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly ILogger<AuthController> _logger;
        private readonly IConfiguration _config;
        private readonly ITokenGenerator _tokenGenerator;

        public AuthController(IUserService userService, ILogger<AuthController> logger , IConfiguration config ,ITokenGenerator tokenGenerator)
        {
            _userService = userService;
            _logger = logger;
            _config = config;
            _tokenGenerator = tokenGenerator;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO user)
        {
            var registrationResponse = await _userService.AddUser(user);

            _logger.LogInformation(registrationResponse);
            return Ok(registrationResponse);
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userCredentials)
        {
            try
            {
                // if the credentials are correct extract the userId 
                
                var userId = await _userService.Login(userCredentials);

                if (string.IsNullOrEmpty(userId)) return BadRequest(new LoginRes{ IsSuccessfull = false, Message = ResponseMessages.IncorrectCredentials });

                // generate the tokens based on the user ID
                var tokens = _tokenGenerator.GetTokens(userId);

                await _userService.SaveRefreshToken(tokens.RefreshToken, userId);

                return Ok(new LoginRes 
                { 
                    IsSuccessfull = true,
                    Message = ResponseMessages.SuccessfulLogin, 
                    JwtTokens = new Tokens()
                    {
                        AccessToken = tokens.AccessToken,
                        RefreshToken = tokens.RefreshToken
                    }
                });
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                return StatusCode(StatusCodes.Status500InternalServerError , new LoginRes { IsSuccessfull = false, Message = ResponseMessages.IncorrectCredentials });
            }
        }


        [HttpPost("refresh-token")]
        public  IActionResult Refresh([FromBody] RefreshTokenRequest rfToken)
        {
            var isValid = _tokenGenerator.IsTokenValid(rfToken.RefreshToken);

            if (!isValid)
            {
                return Unauthorized("Refresh token expired");
            }

            var refreshToken = new JwtSecurityTokenHandler().ReadJwtToken(rfToken.RefreshToken);
            var userId = refreshToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return Unauthorized("Invalid token claims");
            }

            var newTokens = _tokenGenerator.GetTokens(userId);
            //save and replace the new AccessToken
            _userService.SaveRefreshToken(newTokens.RefreshToken, userId);

            return Ok(new Tokens(){AccessToken = newTokens.AccessToken, RefreshToken = newTokens.RefreshToken});
        }

        [HttpGet("logout")]

        public IActionResult Logout()
        {
            
            return Ok(new Tokens() { AccessToken = "", RefreshToken = "" });
        }

    }
}
