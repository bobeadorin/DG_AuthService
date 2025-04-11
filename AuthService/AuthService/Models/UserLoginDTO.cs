using System.ComponentModel.DataAnnotations;

namespace AuthService.Models
{
    public class UserLoginDTO
    {
        [EmailAddress] 
        public string Email { get; set; }
        public string Password { get; set; }
    }
}
