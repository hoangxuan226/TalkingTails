using TalkingTails.Repository.Entities;

namespace TalkingTails.API.Models.Authentication
{
    public class AuthResponse(string accessToken, DateTime accessTokenExpiration , ApplicationUser user, IList<string> roles)
    {
        public string Id { get; set; } = user.Id;
        public string? UserName { get; set; } = user.UserName;
        public string AccessToken { get; set; } = accessToken;
        public DateTime AccessTokenExpiration { get; set; } = accessTokenExpiration;
        public IList<string> Roles { get; set; } = roles;
    }
}
