using Microsoft.AspNetCore.Http;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Business.Models.Pets
{
    public class UpdatePetRequestDto
    {
        public required int Id { get; set; }
        public required string PetName { get; set; }
        public PetSpecies Species { get; set; }
        public required string Breed { get; set; }
        public PetAgeCategories Age { get; set; }
        public double Weight { get; set; }
        public Gender Gender { get; set; }

        public required string Description { get; set; }
        public required string[] LivingEnvironmentNeeds { get; set; }

        public required List<PetInfoItem> Information { get; set; }
        public List<string> ExistingImageUrls { get; set; } = [];
        public ICollection<IFormFile> NewPetImages { get; set; } = [];
        public required string OrganizationId { get; set; }
    }
}