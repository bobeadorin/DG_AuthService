namespace AuthService.Models.AccountValidationModel
{
    public class EmailActivationRequest
    {
        public string Token { get; set; }
        public string ActivateUrl { get; set; }
        public string ToEmail { get; set; }
    }
}
