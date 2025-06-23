using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Organizations
{
    public class AdminOrganListRequestDto
    {
        public int? PageIndex { get; set; }
        public int? PageSize { get; set; }
        public OrganizationStatus? FilterByStatus { get; set; }
        public string? SearchByName { get; set; }
        public string? SearchByEmail { get; set; }
        public string? Sort { get; set; }
    }
}