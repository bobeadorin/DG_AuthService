using AuthService.Helpers.HelpersInterfaces;
using AuthService.Models.ControllerResponses;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Helpers
{
    public class TokenGenerator: ITokenGenerator
    {
        private readonly IConfiguration _config;
        private readonly ILogger<TokenGenerator> _logger;

        public TokenGenerator(IConfiguration config, ILogger<TokenGenerator> logger)
        {
            _config = config;
            _logger = logger;
        }


        public Tokens GetTokens(string userId)
        {

            var accessToken = GenerateJwtToken(expiryInMinutes: _config.GetValue<int>("Jwt:AccessTokenExpiryInMinutes"), userId);
            var refreshToken = GenerateJwtToken(expiryInMinutes: _config.GetValue<int>("Jwt:RefreshTokenExpiryInDays") * 24 * 60, userId);

            return new Tokens(){AccessToken = accessToken, RefreshToken = refreshToken};
        }


        private string GenerateJwtToken(int expiryInMinutes, string userId)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["Jwt:Key"]!));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new Claim[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, userId)
            };

            var token = new JwtSecurityToken(
                issuer: _config["Jwt:Issuer"],
                audience: _config["Jwt:Audience"],
                expires: DateTime.UtcNow.AddMinutes(expiryInMinutes),
                signingCredentials: credentials,
                claims: claims
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public bool IsTokenValid(string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();

            try
            {
                var validationParameters = new TokenValidationParameters
                {
                    ValidateIssuer = false, // Disable issuer validation (since you're using local IP)
                    ValidateAudience = false, // Disable audience validation
                    ValidateLifetime = true, // Still check expiry
                    ValidateIssuerSigningKey = true, // Keep this true to verify the token signature
                    IssuerSigningKey = new SymmetricSecurityKey(
                        Encoding.UTF8.GetBytes(_config["Jwt:Key"]!) // Still validate signing key
                    )
                };

                tokenHandler.ValidateToken(token, validationParameters, out _);
                return true;
            }
            catch (SecurityTokenException)
            {
                return false;
            }
        }

        public Guid GetUserId(string token)
        {
            try
            {
                var tokenHandler = new JwtSecurityTokenHandler();
                var jwtToken = tokenHandler.ReadJwtToken(token);

                // Extract the 'sub' claim (where you stored the userId during generation)
                var userId = jwtToken.Claims.FirstOrDefault(c => c.Type == JwtRegisteredClaimNames.Sub)?.Value;

                if (string.IsNullOrEmpty(userId))
                {
                    throw new SecurityTokenException("User ID claim not found in token");
                }

                return new Guid(userId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }
    


        private static string GenerateRandomEmailToken()
        {
            var randomNumber = new byte[32];
            using var rng = RandomNumberGenerator.Create();

            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber)
                .Replace('+', '-')  
                .Replace('/', '_')  
                .Replace("=", "");  
        }

        public static (string RawToken , string HashedToken, DateTime Expiration) GenerateActivationToken()
        {
            var rawToken = GenerateRandomEmailToken();
            var hashedToken = Hashing.ToSHA256(rawToken);
            var expiration = DateTime.UtcNow.AddMinutes(30); 

            return (rawToken, hashedToken, expiration);
        }
    }
}
