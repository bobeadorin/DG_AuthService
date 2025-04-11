using AuthService.Helpers.HelpersInterfaces;
using AuthService.Models.ControllerResponses;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Runtime.InteropServices.JavaScript;
using System.Security.Claims;
using System.Text;

namespace AuthService.Helpers
{
    public class TokenGenerator: ITokenGenerator
    {
        private IConfiguration _config;

        public TokenGenerator(IConfiguration config)
        {
            _config = config;
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
    }
}
