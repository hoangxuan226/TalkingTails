using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Pets
{
    public class OrganPetBasicDto
    {
        public required int Id { get; set; }
        public required string PetName { get; set; }
        public required PetSpecies Species { get; set; }
        public required PetAgeCategories Age { get; set; }
        public required PetStatus Status { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}