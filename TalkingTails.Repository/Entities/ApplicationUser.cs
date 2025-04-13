using Microsoft.AspNetCore.Identity;

namespace TalkingTails.Repository.Entities
{
    public class ApplicationUser : IdentityUser
    {
        public List<RefreshToken> RefreshTokens { get; set; } = [];
    }
}
