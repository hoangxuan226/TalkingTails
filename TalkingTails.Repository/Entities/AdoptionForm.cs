using TalkingTails.Repository.Constants;

namespace TalkingTails.Repository.Entities
{
    public class AdoptionForm
    {
        public int Id { get; set; }
        public string FormSubmitterId { get; set; }
        public int PetId { get; set; }
        public string FullName { get; set; }
        public string ContactPhone { get; set; }
        public string ContactEmail { get; set; }
        public string ContactAddress { get; set; }
        public string LivingConditions { get; set; }
        public bool HasOtherPets { get; set; }
        public DateTime AvailableContactTime { get; set; }
        public FormStatus Status { get; set; }
        public string RejectReason { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser FormSubmitter { get; set; }
        public Pet Pet { get; set; }
    }
}