using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using AuthService.Models.AccountValidationModel;
using AuthService.Services.Interfaces;
using Azure;

namespace AuthService.Services
{
    public class RequestsService:IRequestsService
    {
        private readonly HttpClient _httpClient;

        public RequestsService(HttpClient httpClient)
        {
            _httpClient = httpClient ?? throw new ArgumentNullException(nameof(httpClient));
        }

        public async Task<HttpResponseMessage> SendRegistrationToken(string uri, EmailActivationRequest body )
        {
            var jsonBody = JsonSerializer.Serialize(body);
            var content = new StringContent(jsonBody, Encoding.UTF8, "application/json");

            var response = await _httpClient.PostAsync(uri, content);

            response.EnsureSuccessStatusCode();

            return response;
        }
    }
}
