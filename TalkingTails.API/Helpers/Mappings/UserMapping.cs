using TalkingTails.API.Models.Users;
using TalkingTails.Business.Models.Organizations;
using TalkingTails.Business.Models.Users;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class UserMapping
    {
        public static EditCustomerRequestDto ToEditCustomerRequestDto(this EditCustomerRequest request, string userId)
        {
            return new EditCustomerRequestDto
            {
                Id = userId,
                Name = request.Name,
                Address = request.Address,
                Birthday = request.Birthday,
                PhoneNumber = request.PhoneNumber,
                Email = request.Email
            };
        }

        public static AdminUserListRequestDto ToListRequestDto(this AdminUserListRequest request)
        {
            return new AdminUserListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                SearchByName = request.SearchByName,
                SearchByEmail = request.SearchByEmail,
                Sort = request.Sort
            };
        }

        public static UpdateRequestDto ToOrganUpdateRequestDto(this EditOrganizationRequest request, string id)
        {
            return new UpdateRequestDto
            {
                Id = id,
                ProfileImage = request.ProfileImage,
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Address = request.Address,
                Description = request.Description,
                MeetLink = request.MeetLink
            };
        }
    }
}