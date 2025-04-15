using System.ComponentModel.DataAnnotations;

namespace AuthService.Models.AccountValidationModel
{
    public class AccountValidationTokens
    {
        [Key]
        public Guid Id { get; set; }
        public string ActivationToken { get; set; }
        public Guid UserId { get; set; }
        public DateTime ExpirationDateTime { get; set; }
    }
}
