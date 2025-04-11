using AuthService.Constant;
using AuthService.DbConnection;
using AuthService.Helpers;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<UserService> _logger;

        public UserService(AppDbContext dbContext, ILogger<UserService> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }


        public async Task<List<User>> GetAllUsers()
        {
            var users =  await _dbContext.Users.ToListAsync();

            return users;
        }

        public async Task<string> Login(UserLoginDTO userAccount)
        {
            try
            {
                var user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == userAccount.Email && u.Password == Hashing.toSHA256(userAccount.Password));

                if (user == null) return string.Empty;

                return user.Id.ToString();
            }
            catch(Exception ex)
            {
                _logger.LogError(ex.Message);
                throw;
            }
        }

        public async Task SaveRefreshToken(string rfToken ,string id)
        {
            var user = _dbContext.Users.FirstOrDefault(u => u.Id == Guid.Parse(id));

            if (user == null) return;

            user.RefreshToken = rfToken;
            
            await _dbContext.SaveChangesAsync();
        }

        public async Task<string> AddUser(UserRegistrationDTO user)
        {
            try
            {
                var existingUser = _dbContext.Users.FirstOrDefault((x) => x.Email == user.Email || x.Name == user.Name);

                if (existingUser != null && existingUser.Name == user.Name)
                {
                    _logger.LogInformation(ResponseMessages.UsernameAlreadyExists);
                    return ResponseMessages.UsernameAlreadyExists;

                }

                if (existingUser != null && existingUser.Email == user.Email)
                {
                    _logger.LogInformation(ResponseMessages.EmailAlreadyUsed);
                    return ResponseMessages.EmailAlreadyUsed;
                }

                _dbContext.Users.Add(
                    new User
                    {
                        Email = user.Email,
                        Name = user.Name,
                        Password = Hashing.toSHA256(user.Password),
                        Age = user.Age,
                        ProfileData = new ProfileData
                        {
                            Points = 100,
                            Status = "Noob",
                            Followers = new List<Guid>(),
                            Posts = new List<Guid>(),
                            Groups = new List<Guid>()
                        },
                        ProfileDataId = Guid.NewGuid(),
                        RefreshToken = string.Empty,
                    }
                );

                await _dbContext.SaveChangesAsync();

                _logger.LogInformation(ResponseMessages.UserSuccessfullyRegistered);
                return ResponseMessages.UserSuccessfullyRegistered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message);
                return ResponseMessages.UserRegistrationFailed;
            }

        }
    }
}
