namespace TalkingTails.API.Models.Authentication
{
    public class AuthResponse
    {
        public required string Id { get; set; }
        public required string UserName { get; set; }
        public required string AccessToken { get; set; }
        public required IList<string> Roles { get; set; }
    }
}
