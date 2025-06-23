using TalkingTails.API.Models.Authentication;
using TalkingTails.Business.Models.Authentication;
using TalkingTails.Repository.Entities;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class AuthMapping
    {
        public static AuthResponse ToAuthResponse(this AuthDto authDto)
        {
            return new AuthResponse
            {
                Id = authDto.User.Id,
                UserName = authDto.User.UserName ?? "User",
                AccessToken = authDto.AccessToken,
                Roles = authDto.Roles
            };
        }

        public static ApplicationUser ToApplicationUser(this RegisterRequest request)
        {
            return new ApplicationUser
            {
                UserName = request.Email,
                Email = request.Email,
                Name = request.Email
            };
        }
    }
}