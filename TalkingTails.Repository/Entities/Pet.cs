using TalkingTails.Repository.Constants;

namespace TalkingTails.Repository.Entities
{
    public class Pet
    {
        public int Id { get; set; }
        public string PetName { get; set; }
        public PetSpecies Species { get; set; }
        public string Breed { get; set; }
        public PetAgeCategories Age { get; set; }
        public double Weight { get; set; }
        public Gender Gender { get; set; }

        public string Description { get; set; }
        public string[] LivingEnvironmentNeeds { get; set; }
        public List<PetInfoItem> Information { get; set; }

        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? LastInterviewed { get; set; }
        public PetStatus Status { get; set; }
        public string OrganizationId { get; set; }
        public ApplicationUser Organization { get; set; }
        public ICollection<PetImage> PetImages { get; set; }
        public ICollection<AdoptionForm> AdoptionForms { get; set; }
        public ICollection<AdoptedPet> AdoptedPets { get; set; }
    }

    public class PetInfoItem
    {
        public string Label { get; set; } // "Triệt sản", "Đi vệ sinh đúng chỗ"
        public bool Value { get; set; } // true / false
    }
}