using System.Linq.Expressions;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.AdoptedPets
{
    public class CreateAdoptedPetDto : IMappable<InterviewSchedule>
    {
        public int PetId { get; set; }
        public required string AdopterId { get; set; }
        public int AdoptionFormId { get; set; }

        public static Dictionary<string, Expression<Func<InterviewSchedule, object>>> Mappings { get; } = new()
        {
            { nameof(PetId), ic => ic.AdoptionForm.Pet.Id },
            { nameof(AdopterId), ic => ic.AdoptionForm.FormSubmitterId },
            { nameof(AdoptionFormId), ic => ic.AdoptionFormId }
        };
    }
}