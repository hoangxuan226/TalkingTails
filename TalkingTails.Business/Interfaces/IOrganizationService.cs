using TalkingTails.Business.Models.Organizations;

namespace TalkingTails.Business.Interfaces
{
    public interface IOrganizationService
    {
        Task<List<OrganizationBasicDto>> GetAllAsync();
    }
}