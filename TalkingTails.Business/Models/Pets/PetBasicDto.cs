using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Pets
{
    public class PetBasicDto : IMappable<Pet>
    {
        public required int Id { get; set; }
        public required string PetName { get; set; }
        public required string Slug { get; set; }
        public required PetAgeCategories Age { get; set; }
        public required string Breed { get; set; }
        public required Gender Gender { get; set; }
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