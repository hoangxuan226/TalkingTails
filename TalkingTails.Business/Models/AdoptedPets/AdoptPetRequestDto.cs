namespace TalkingTails.Business.Models.AdoptedPets
{
    public class AdoptPetRequestDto
    {
        public int InterviewScheduleId { get; set; }
        public required string OrganizationId { get; set; }
    }
}