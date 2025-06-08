using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Pets
{
    public class GuestPetDetailsDto : IMappable<Pet>
    {
        public required int Id { get; set; }
        public required string PetName { get; set; }
        public required PetSpecies Species { get; set; }
        public required string Breed { get; set; }
        public required PetAgeCategories Age { get; set; }
        public required double Weight { get; set; }
        public required Gender Gender { get; set; }

        public required string Description { get; set; }
        public required string[] LivingEnvironmentNeeds { get; set; }
        public required List<PetInfoItem> Information { get; set; }

        public required string Slug { get; set; }
        public required string[] ImageUrls { get; set; }

        public static Dictionary<string, Expression<Func<Pet, object>>> Mappings { get; } = new()
        {
            {
                nameof(ImageUrls),
                x => x.PetImages.Select(pi => pi.ImageUrl).ToArray()
            }
        };
    }
}