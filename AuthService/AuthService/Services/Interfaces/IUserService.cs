using AuthService.Models;

namespace AuthService.Services.Interfaces
{
    public interface IUserService
    {
        public Task<string> AddUser(UserRegistrationDTO user);
        public Task<List<User>> GetAllUsers();
        public Task<string> Login(UserLoginDTO userAccount);
        public Task SaveRefreshToken(string rfToken, string id);
    }
}
