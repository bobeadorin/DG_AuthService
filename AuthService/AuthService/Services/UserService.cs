using AuthService.Constant;
using AuthService.DbConnection;
using AuthService.Helpers;
using AuthService.Models;
using AuthService.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

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

        public async Task<string> AddUser(User user)
        {
            try
            {
                var existingUser = _dbContext.Users.FirstOrDefault((x) => x.Email == user.Email || x.Name == user.Name);

                if (existingUser != null && existingUser.Name == user.Name) return ResponseMessages.UsernameAlreadyExists;
                
                if (existingUser != null && existingUser.Email == user.Email) return ResponseMessages.EmailAlreadyUsed;

                _dbContext.Users.Add(
                    new User
                    {
                        Email = user.Email,
                        Name = user.Name,
                        Password = user.Password,
                        Age = user.Age,
                        ProfileData = new ProfileData
                        {
                            Points = 100,
                            Status = "Noob",
                            Followers = new List<Guid>(),
                            Posts = new List<Guid>(),
                            Groups = new List<Guid>()
                        }
                    }
            );

                await _dbContext.SaveChangesAsync();
                return ResponseMessages.UserSuccefullyRegistered;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "An error occurred while adding a user.");
                return ResponseMessages.UserRegistrationFailed;
            }

        }
    }
}
