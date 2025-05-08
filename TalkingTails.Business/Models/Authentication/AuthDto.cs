using TalkingTails.Repository.Entities;

namespace TalkingTails.Business.Models.Authentication
{
    public class AuthDto
    {
        public required string AccessToken { get; set; }
        public required string RefreshToken { get; set; }
        public required ApplicationUser User { get; set; }
        public required IList<string> Roles { get; set; }
    }
}
