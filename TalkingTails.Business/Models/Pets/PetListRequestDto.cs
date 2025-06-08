using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Pets
{
    public class PetListRequestDto
    {
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
        public required string? SearchByName { get; set; }
        public required PetSpecies? FilterBySpecies { get; set; }
        public required string? Sort { get; set; }
        public required PetStatus? FilterByStatus { get; set; }
        public required PetAgeCategories? FilterByAge { get; set; }
    }
}