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
    }
}