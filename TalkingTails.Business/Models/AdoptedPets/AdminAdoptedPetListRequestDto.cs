namespace TalkingTails.Business.Models.AdoptedPets
{
    public class AdminAdoptedPetListRequestDto
    {
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
        public DateTime? FilterByStartDate { get; set; }
        public DateTime? FilterByEndDate { get; set; }
        public required string? Sort { get; set; }
    }
}