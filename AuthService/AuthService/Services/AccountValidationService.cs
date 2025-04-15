using AuthService.DbConnection;
using AuthService.Models.AccountValidationModel;
using AuthService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AuthService.Services
{
    public class AccountValidationService: IAccountValidationService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<AccountValidationService> _logger;

        public AccountValidationService(AppDbContext dbContext, ILogger<AccountValidationService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task SaveTokens(Guid userId, string hashedToken, DateTime expirationDateTime)
        {
            try
            {
                await _dbContext.AccountValidationTokens.AddAsync(new AccountValidationTokens
                {
                    Id = Guid.NewGuid(),
                    ActivationToken = hashedToken,
                    ExpirationDateTime = expirationDateTime,
                    UserId = userId
                });

                await _dbContext.SaveChangesAsync();
                _logger.LogInformation("Tokens saved");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex.Message);

                throw;
            }
        }

        public async Task<Guid> GetUserByActivationToken(string token)
        {
            var isUser = await _dbContext.AccountValidationTokens.FirstOrDefaultAsync(u => u.ActivationToken == token);

            if (isUser is null) return Guid.Empty;

            return isUser.UserId;
        }


            
        
    }
}
