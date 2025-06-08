using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.AdoptionForms
{
    public class FormListRequestDto
    {
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
        public required FormStatus? FilterByStatus { get; set; }
        public required string? Sort { get; set; }
    }
}