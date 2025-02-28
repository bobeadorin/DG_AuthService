using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace AuthService.Models
{
    public class User
    {
        [Key]
        [JsonIgnore]
        public Guid Id { get; set; }
        public string Email { get; set; }
        public string Name { get; set; }
        public int Age{ get; set; }
        public string Password { get; set; }

        [JsonIgnore]
        public Guid ProfileDataId { get; set; }
        [JsonIgnore]
        public ProfileData? ProfileData { get; set; }

    }
}
