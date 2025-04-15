using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthService.Models.UserModels
{
    public class User
    {
        [Key]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Age{ get; set; }
        public string Password { get; set; }

        public Guid ProfileDataId { get; set; }
        public ProfileData? ProfileData { get; set; }
        public string RefreshToken { get; set; }

        public bool IsActivated { get; set; }
        public DateTime? ActivatedAt { get; set; }

    }
}
