using TalkingTails.API.Models.Organizations;
using TalkingTails.Repository.Entities;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class OrganizationMapping
    {
        public static ApplicationUser ToApplicationUser(this CreateRequest request)
        {
            return new ApplicationUser()
            {
                UserName = request.Email,
                Email = request.Email
            };
        }

        public static OrganizationResponse ToOrganizationResponse(this ApplicationUser user)
        {
            return new OrganizationResponse
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
            };
        }
    }
}