using AuthService.Models.AccountValidationModel;

namespace AuthService.Services.Interfaces
{
    public interface IRequestsService
    {
        public Task<HttpResponseMessage> SendRegistrationToken(string uri, EmailActivationRequest body);
    }
}
