using TalkingTails.API.Models.Organizations;
using TalkingTails.Business.Models.Organizations;
using TalkingTails.Repository.Entities;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class OrganizationMapping
    {
        public static OrganizationResponse ToOrganizationResponse(this ApplicationUser user)
        {
            return new OrganizationResponse
            {
                Id = user.Id,
                Email = user.Email ?? string.Empty,
            };
        }

        public static AdminOrganListRequestDto ToAdminOrganListRequestDto(this AdminOrganListRequest request)
        {
            return new AdminOrganListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                FilterByStatus = request.FilterByStatus,
                SearchByName = request.SearchByName,
                SearchByEmail = request.SearchByEmail,
                Sort = request.Sort
            };
        }

        public static CreateRequestDto ToCreateRequestDto(this CreateRequest request)
        {
            return new CreateRequestDto
            {
                ProfileImage = request.ProfileImage,
                Name = request.Name,
                Email = request.Email,
                PhoneNumber = request.PhoneNumber,
                Status = request.Status,
                Address = request.Address,
                Description = request.Description,
                Password = request.Password,
            };
        }

        public static UpdateRequestDto ToUpdateRequestDto(this UpdateRequest request)
        {
            return new UpdateRequestDto
            {
                Id = request.Id,
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