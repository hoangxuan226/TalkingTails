using TalkingTails.API.Models.AdoptedPets;
using TalkingTails.Business.Models.AdoptedPets;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class AdoptedPetMapping
    {
        public static AdoptPetRequestDto ToAdoptPetRequestDto(this AdoptPetRequest request, string organizationId)
        {
            return new AdoptPetRequestDto
            {
                InterviewScheduleId = request.InterviewScheduleId,
                OrganizationId = organizationId
            };
        }

        public static AdminAdoptedPetListRequestDto ToAdminAdoptedPetListRequestDto(
            this AdminAdoptedPetListRequest request)
        {
            return new AdminAdoptedPetListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                FilterByStartDate = request.FilterByStartDate,
                FilterByEndDate = request.FilterByEndDate,
                Sort = request.Sort,
            };
        }
    }
}