using System.IdentityModel.Tokens.Jwt;
using System.Runtime.CompilerServices;
using AuthService.Constant;
using AuthService.Helpers;
using AuthService.Helpers.HelpersInterfaces;
using AuthService.Models.AccountValidationModel;
using AuthService.Models.ControllerResponses;
using AuthService.Models.JwtTokensModels;
using AuthService.Models.UserModels;
using AuthService.Services.Interfaces;
using Azure.Core;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
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
        private readonly ITokenGenerator _tokenGenerator;
        private readonly IAccountValidationService _accountValidationService;
        private readonly IRequestsService _requestsService;

        public AuthController(IUserService userService, ILogger<AuthController> logger  ,ITokenGenerator tokenGenerator, IAccountValidationService accountValidationService, IRequestsService requestsService)
        {
            _userService = userService;
            _logger = logger;
            _tokenGenerator = tokenGenerator;
            _accountValidationService = accountValidationService;
            _requestsService = requestsService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] UserRegistrationDTO user)
        {
            var registrationResponse = await _userService.AddUser(user);

            if (registrationResponse == ResponseMessages.UserSuccessfullyRegistered)
            {
                var (rawToken,hashedToken, expiration) = TokenGenerator.GenerateActivationToken();

                var userId = await _userService.GetUserByEmail(user.Email);

                if (userId.Equals(Guid.Empty) ) return BadRequest("User has not been registered");

                await _accountValidationService.SaveTokens(userId, hashedToken, expiration);

                var activationRequestBody = new EmailActivationRequest()
                {
                    ActivateUrl = ApiEndpoints.ActivateAccount(rawToken),
                    Token = rawToken,
                    ToEmail = user.Email
                };
                await _requestsService.SendRegistrationToken(ApiEndpoints.SendRegistrationMail, activationRequestBody);
            }

            _logger.LogInformation(registrationResponse);
            return Ok(registrationResponse);

        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] UserLoginDTO userCredentials)
        {
            try
            {
                // if the credentials are correct extract the userId 
                
                var (user, isActivated) = await _userService.Login(userCredentials);

                if (user is null) return BadRequest(new LoginRes{ IsSuccessfull = false, Message = ResponseMessages.IncorrectCredentials });

                if (!isActivated)
                    return BadRequest(new LoginRes { IsSuccessfull = false, Message = ResponseMessages.AccountNotActive });
                // generate the tokens based on the user ID
                var tokens = _tokenGenerator.GetTokens(user.Id.ToString());

                await _userService.SaveRefreshToken(tokens.RefreshToken, user.Id.ToString());

                return Ok(new LoginRes 
                { 
                    IsSuccessfull = true,
                    Message = ResponseMessages.SuccessfulLogin, 
                    JwtTokens = new Tokens()
                    {
                        AccessToken = tokens.AccessToken,
                        RefreshToken = tokens.RefreshToken
                    },
                    UserData =new UserDTO()
                    {
                        Id = user.Id,
                        Name = user.Name,
                        Email = user.Email,
                        Age = user.Age,
                        ProfileData = user.ProfileData
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


        [HttpGet("/activate/{token}")]
        public async Task<IActionResult> ActivateAccount([FromRoute] string token)
        {
            if (string.IsNullOrEmpty(token)) return BadRequest("Invalid token");

            var getUserIdBasedOnValidToken = await _accountValidationService.GetUserByActivationToken(Hashing.ToSHA256(token));

            if(getUserIdBasedOnValidToken.Equals(Guid.Empty)) return NotFound();

            bool isActivated = await _userService.ActivateAccount(getUserIdBasedOnValidToken);

            if (!isActivated) return BadRequest("Your account has not beed activated");

            return Ok("Your account has been activated");

        }
    }
}
