using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Pets
{
    public class InterviewedPetListRequestDto
    {
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
        public PetSpecies? FilterBySpecies { get; set; }
        public PetAgeCategories? FilterByAge { get; set; }
        public required string? SearchByName { get; set; }
    }
}