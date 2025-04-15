namespace AuthService.Constant
{
    public class ApiEndpoints
    {
        public const string SendRegistrationMail = BaseEnpoints.MailService + "/SendRegistrationMail";
        public static string ActivateAccount(string token) => BaseEnpoints.AuthService + "/activate/";
    }

    public static class BaseEnpoints
    {
        public const string MailService = "http://192.168.1.105:5219";
        public const string AuthService = "http://192.168.1.105:5204";
    }

}


