using TalkingTails.Repository.Constants;

namespace TalkingTails.Repository.Entities
{
    public class Pet
    {
        public int Id { get; set; }
        public string PetName { get; set; }
        public PetSpecies Species { get; set; }
        public string Breed { get; set; }
        public int Age { get; set; }
        public double Weight { get; set; }
        public Gender Gender { get; set; }

        public string Description { get; set; }

        // ["Phòng cách sống năng động", "Đi dạo thường xuyên", "Thời gian vui gia đình"]
        public string[] LivingEnvironmentNeeds { get; set; }

        // Stored as JSON ({"Tình trạng": [{"Text": "Triệt sản", "Icon": "Check"}]})
        public string Information { get; set; }

        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public PetStatus Status { get; set; }

        public ApplicationUser Owner { get; set; }
        public ICollection<PetImage> PetImages { get; set; }
        public ICollection<AdoptionForm> AdoptionForms { get; set; }
        public ICollection<AdoptedPet> AdoptedPets { get; set; }
    }
}