using AuthService.Models;

namespace AuthService.Services.Interfaces
{
    public interface IUserService
    {
        public Task<string> AddUser(User user);
        public Task<List<User>> GetAllUsers();
    }
}
