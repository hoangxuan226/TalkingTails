using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.AdoptionForms
{
    public class OrganizationFormBasicDto
    {
        public int Id { get; set; }
        public int PetName { get; set; }
        public PetSpecies Species { get; set; }
        public string FullName { get; set; }
        public string ContactPhone { get; set; }
        public DateTime AvailableContactTime { get; set; }
    }
}