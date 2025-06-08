using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Pets
{
    public class InterviewedPetDto : IMappable<Pet>
    {
        public int Id { get; set; }
        public required string PetName { get; set; }
        public PetSpecies Species { get; set; }
        public required string Breed { get; set; }
        public PetAgeCategories Age { get; set; }
        public required string ImageUrl { get; set; }

        public static Dictionary<string, Expression<Func<Pet, object>>> Mappings { get; } = new()
        {
            {
                nameof(ImageUrl),
                x => x.PetImages.FirstOrDefault() != null ? x.PetImages.FirstOrDefault()!.ImageUrl : string.Empty
            }
        };
    }
}