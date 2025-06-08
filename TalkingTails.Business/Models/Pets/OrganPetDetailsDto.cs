using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Pets
{
    public class OrganPetDetailsDto : IMappable<Pet>
    {
        public int Id { get; set; }
        public required string PetName { get; set; }
        public PetSpecies Species { get; set; }
        public required string Breed { get; set; }
        public PetAgeCategories Age { get; set; }
        public double Weight { get; set; }
        public Gender Gender { get; set; }
        public required string Description { get; set; }
        public required string[] LivingEnvironmentNeeds { get; set; }
        public required List<PetInfoItem> Information { get; set; }
        public required string Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastInterviewed { get; set; }
        public PetStatus Status { get; set; }
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