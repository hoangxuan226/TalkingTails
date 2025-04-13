using TalkingTails.Repository.Entities;

namespace TalkingTails.Business.Models.Authentication
{
    public class AuthDto(string accessToken, DateTime accessTokenExpiration, string refreshToken, ApplicationUser user, IList<string> roles)
    {
        public string AccessToken { get; set; } = accessToken;
        public DateTime AccessTokenExpiration { get; set; } = accessTokenExpiration;
        public string RefreshToken { get; set; } = refreshToken;
        public ApplicationUser User { get; set; } = user;
        public IList<string> Roles { get; set; } = roles;
    }
}
