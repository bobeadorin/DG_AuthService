using AuthService.Models.UserModels;

namespace AuthService.Models.ControllerResponses
{
    public class LoginRes
    {
        public bool IsSuccessfull{ get; set; }
        public string Message { get; set; }
        public Tokens JwtTokens { get; set; }
        public UserDTO UserData { get; set; }
    }

    public class Tokens
    {
        public string AccessToken { get; set; }
        public string RefreshToken { get; set; }

    }
}
