namespace TalkingTails.Repository.Entities
{
    public class AdoptedPet
    {
        public int Id { get; set; }
        public int PetId { get; set; }
        public string AdopterId { get; set; }
        public int AdoptionFormId { get; set; }
        public DateTime AdoptionDate { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public Pet Pet { get; set; }
        public ApplicationUser Adopter { get; set; }
        public AdoptionForm AdoptionForm { get; set; }
    }
}