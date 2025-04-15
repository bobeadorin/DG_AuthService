using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.UserModels
{
    public class UserDTO
    {
        public string Email { get; set; }
        public string Name { get; set; }
        public int Age { get; set; }
        public ProfileData? ProfileData { get; set; }
    }
}
