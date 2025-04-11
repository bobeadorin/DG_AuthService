using AuthService.Models.ControllerResponses;

namespace AuthService.Helpers.HelpersInterfaces
{
    public interface ITokenGenerator
    {
        public Tokens GetTokens(string userId);
        public bool IsTokenValid(string token);
    }
}
