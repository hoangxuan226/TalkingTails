using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.AdoptedPets
{
    public class CusAdoptedPetBasicDto : IMappable<AdoptedPet>
    {
        public required string PetName { get; set; }
        public PetSpecies Species { get; set; }
        public required string Breed { get; set; }
        public PetAgeCategories Age { get; set; }
        public double Weight { get; set; }
        public Gender Gender { get; set; }
        public DateTime AdoptedDate { get; set; }

        public static Dictionary<string, Expression<Func<AdoptedPet, object>>> Mappings { get; } = new()
        {
            { nameof(PetName), ap => ap.Pet.PetName },
            { nameof(Species), ap => ap.Pet.Species },
            { nameof(Breed), ap => ap.Pet.Breed },
            { nameof(Age), ap => ap.Pet.Age },
            { nameof(Weight), ap => ap.Pet.Weight },
            { nameof(Gender), ap => ap.Pet.Gender },
            { nameof(AdoptedDate), ap => ap.CreatedAt }
        };
    }
}