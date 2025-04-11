using Microsoft.Identity.Client;

namespace AuthService.Constant
{
    public class ResponseMessages
    {
        public const string UserSuccessfullyRegistered = "Successfully Registered";
        public const string UsernameAlreadyExists = "Username already exists";
        public const string EmailAlreadyUsed = "There is already an account with that email adress";
        public const string UserRegistrationFailed = "User registration failed";

        public const string SuccessfulLogin = "Succefully logged in";
        public const string IncorrectCredentials = "Incorect username or password";

    }
}
