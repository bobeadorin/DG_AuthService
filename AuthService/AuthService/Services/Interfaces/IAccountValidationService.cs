namespace AuthService.Services.Interfaces
{
    public interface IAccountValidationService
    {
        public Task SaveTokens(Guid userId, string hashedToken, DateTime expirationDateTime);
        public Task<Guid> GetUserByActivationToken(string token);
    }
}
