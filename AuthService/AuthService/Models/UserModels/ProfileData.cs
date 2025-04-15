namespace AuthService.Models.UserModels
{
    public class ProfileData
    {
        public int Points { get; set; }
        public string? Status { get; set; }
        public List<Guid>? Followers { get; set; }
        public List<Guid>? Posts { get; set; }
        public List<Guid>? Groups { get; set; }
    }
}
