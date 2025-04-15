using AuthService.Models.UserModels;

namespace AuthService.Services.Interfaces
{
    public interface IUserService
    {
        public Task<string> AddUser(UserRegistrationDTO user);
        public Task<List<User>> GetAllUsers();
        public Task<(UserDTO? userData, bool isUser)> GetUserById(Guid id);
        public Task<(User? UserData, bool IsActivated)> Login(UserLoginDTO userAccount);
        public Task SaveRefreshToken(string rfToken, string id);
        public Task<Guid> GetUserByEmail(string email);
        public Task<bool> ActivateAccount(Guid userId);

    }
}
