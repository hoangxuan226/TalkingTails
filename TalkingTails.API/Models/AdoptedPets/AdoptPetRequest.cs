using System.ComponentModel.DataAnnotations;

namespace TalkingTails.API.Models.AdoptedPets
{
    public class AdoptPetRequest
    {
        [Required] public int InterviewScheduleId { get; set; }
    }
}