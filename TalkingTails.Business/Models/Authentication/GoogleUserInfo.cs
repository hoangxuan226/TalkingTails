namespace TalkingTails.Business.Models.Authentication
{
    public class GoogleUserInfo
    {
        public required string Email { get; set; }
        public required string Name { get; set; }
        public string? Picture { get; set; }
        public required string GoogleId { get; set; }
    }
}