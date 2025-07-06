using System.ComponentModel.DataAnnotations;

namespace TalkingTails.API.Models.AdoptedPets
{
    public class AdminAdoptedPetListRequest
    {
        [Range(1, int.MaxValue)] public int? PageIndex { get; set; }
        [Range(1, 50)] public int? PageSize { get; set; }
        public DateTime? FilterByStartDate { get; set; }
        public DateTime? FilterByEndDate { get; set; }
        public string? Sort { get; set; }
    }
}