using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.AdoptionForms
{
    public class CustomerFormBasicDto : IMappable<AdoptionForm>
    {
        public required int Id { get; set; }
        public required string PetName { get; set; }
        public required PetSpecies Species { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required FormStatus Status { get; set; }
        public required DateTime UpdatedAt { get; set; }

        public static Dictionary<string, Expression<Func<AdoptionForm, object>>> Mappings { get; } = new()
        {
            { nameof(PetName), form => form.Pet.PetName },
            { nameof(Species), form => form.Pet.Species },
        };
    }
}