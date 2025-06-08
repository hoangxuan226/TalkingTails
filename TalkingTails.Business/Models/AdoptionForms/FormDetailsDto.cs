using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.AdoptionForms
{
    public class FormDetailsDto : IMappable<AdoptionForm>
    {
        public int Id { get; set; }
        public string FormSubmitterId { get; set; }
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

        public string OrganizationId { get; set; }
        public string PetName { get; set; }
        public PetSpecies Species { get; set; }
        public string Breed { get; set; }
        public PetAgeCategories Age { get; set; }
        public double Weight { get; set; }
        public Gender PetGender { get; set; }
        public string PetImageUrl { get; set; }

        public static Dictionary<string, Expression<Func<AdoptionForm, object>>> Mappings { get; } = new()
        {
            {
                nameof(OrganizationId), x => x.Pet.OrganizationId
            },
            {
                nameof(PetName), x => x.Pet.PetName
            },
            {
                nameof(Species), x => x.Pet.Species
            },
            {
                nameof(Breed), x => x.Pet.Breed
            },
            {
                nameof(Age), x => x.Pet.Age
            },
            {
                nameof(Weight), x => x.Pet.Weight
            },
            {
                nameof(PetGender), x => x.Pet.Gender
            },
            {
                nameof(PetImageUrl),
                x => x.Pet.PetImages.FirstOrDefault() != null
                    ? x.Pet.PetImages.FirstOrDefault()!.ImageUrl
                    : string.Empty
            }
        };
    }
}