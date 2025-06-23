using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.Organizations;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IOrganizationService
    {
        Task<List<OrganizationBasicDto>> GetAllAsync();
        Task<Pagination<AdminOrganBasicDto>> GetAllForAdminAsync(AdminOrganListRequestDto requestDto);
        Task<OrganizationDetailsDto?> GetDetailsAsync(string id);
        Task<OneOf<bool, IError>> CreateAsync(CreateRequestDto requestDto);
        Task<OneOf<bool, IError>> UpdateAsync(UpdateRequestDto requestDto);
        Task<OrganizationCountDto> GetOrganizationCountAsync();
    }
}