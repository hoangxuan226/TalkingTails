using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.AdoptedPets
{
    public class AdminAdoptedPetBasicDto : IMappable<AdoptedPet>
    {
        public required string PetName { get; set; }
        public PetSpecies Species { get; set; }
        public required string Breed { get; set; }
        public required string AdopterName { get; set; }
        public DateTime AdoptedDate { get; set; }
        public required string OrganizationName { get; set; }

        public static Dictionary<string, Expression<Func<AdoptedPet, object>>> Mappings { get; } = new()
        {
            { nameof(PetName), ap => ap.Pet.PetName },
            { nameof(Species), ap => ap.Pet.Species },
            { nameof(Breed), ap => ap.Pet.Breed },
            { nameof(AdopterName), ap => ap.Adopter.Name ?? ap.Adopter.UserName ?? "" },
            { nameof(AdoptedDate), ap => ap.CreatedAt },
            { nameof(OrganizationName), ap => ap.Pet.Organization.Name ?? "" }
        };
    }
}