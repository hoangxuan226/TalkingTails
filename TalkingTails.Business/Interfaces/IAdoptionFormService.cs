using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.AdoptionForms;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IAdoptionFormService
    {
        Task<OneOf<bool, IError>> SubmitAdoptionFormAsync(AdoptionForm adoptionForm, string submitterId);
        Task<List<CustomerFormBasicDto>> GetAdoptionFormsByUserIdAsync(string submitterId);
        Task<OneOf<FormDetailsDto, IError>> GetAdoptionFormDetailsByIdAsync(int formId, string requestUserId);
        Task<bool> CancelAdoptionFormAsync(int formId, string userId);
        Task<OneOf<bool, IError>> UpdateAdoptionFormAsync(AdoptionForm adoptionForm, string submitterId);

        Task<Pagination<OrganFormBasicDto>> GetAdoptFormsForOrganizationAsync(FormListRequestDto requestDto,
            string organizationId);

        Task<OneOf<bool, IError>> ApproveAdoptionFormAsync(int formId, string organizationId);
        Task<bool> RejectAdoptionFormAsync(int formId, string organizationId, string rejectReason);
    }
}