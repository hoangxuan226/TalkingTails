using System.Linq.Expressions;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.AdoptionForms;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class AdoptionFormService(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider) : IAdoptionFormService
    {
        public async Task<OneOf<bool, IError>> SubmitAdoptionFormAsync(AdoptionForm adoptionForm, string submitterId)
        {
            try
            {
                if (adoptionForm.AvailableContactTime < dateTimeProvider.UtcNow.AddDays(1))
                {
                    return new InvalidResourcesError
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "AvailableContactTime", ["Thời gian liên hệ phải sau thời điểm gửi đơn ít nhất 1 ngày."] }
                        }
                    };
                }

                adoptionForm.FormSubmitterId = submitterId;
                adoptionForm.Status = FormStatus.Pending;
                adoptionForm.CreatedAt = dateTimeProvider.UtcNow;
                adoptionForm.UpdatedAt = adoptionForm.CreatedAt;
                await unitOfWork.GenericRepository<AdoptionForm>().InsertAsync(adoptionForm);
                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi gửi đơn nhận nuôi.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<List<CustomerFormBasicDto>> GetAdoptionFormsByUserIdAsync(string submitterId)
        {
            // Update forms that are pending and older than 24 hours to under review
            await UpdateNoLongerEditableForms(submitterId);

            Expression<Func<AdoptionForm, bool>> filter = form => form.FormSubmitterId.Equals(submitterId);
            var sort = "CreatedAt desc";
            return await unitOfWork.GenericRepository<AdoptionForm>()
                .GetAllAsync<CustomerFormBasicDto>(filter, sort);
        }

        public async Task<OneOf<FormDetailsDto, IError>> GetAdoptionFormDetailsByIdAsync(int formId,
            string requestUserId)
        {
            var form = await unitOfWork.GenericRepository<AdoptionForm>()
                .GetAsync<FormDetailsDto>(f => f.Id == formId);
            if (form == null) return new NotFoundError();
            if (form.FormSubmitterId != requestUserId && form.OrganizationId != requestUserId)
                return new ForbiddenError();
            return form;
        }

        public async Task<bool> CancelAdoptionFormAsync(int formId, string userId)
        {
            var result = await unitOfWork.GenericRepository<AdoptionForm>()
                .ExecuteUpdateAsync(f => f.Id == formId && f.FormSubmitterId.Equals(userId),
                    update => update.SetProperty(f => f.Status, FormStatus.Canceled));
            return result > 0;
        }

        public async Task<OneOf<bool, IError>> UpdateAdoptionFormAsync(AdoptionForm updatedForm, string submitterId)
        {
            var savedForm = await unitOfWork.GenericRepository<AdoptionForm>()
                .GetAsync(f => f.Id == updatedForm.Id);
            if (savedForm == null) return new NotFoundError();
            if (savedForm.FormSubmitterId != submitterId) return new ForbiddenError();
            if (!savedForm.Status.Equals(FormStatus.Pending))
                return new InvalidResourcesError
                {
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Status", ["Chỉ có đơn đang chờ xử lý mới có thể cập nhật."] }
                    }
                };
            try
            {
                if (updatedForm.AvailableContactTime < savedForm.CreatedAt.AddDays(1))
                {
                    return new InvalidResourcesError
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            { "AvailableContactTime", ["Thời gian liên hệ phải sau thời điểm tạo đơn ít nhất 1 ngày."] }
                        }
                    };
                }

                savedForm.FullName = updatedForm.FullName;
                savedForm.ContactPhone = updatedForm.ContactPhone;
                savedForm.ContactEmail = updatedForm.ContactEmail;
                savedForm.ContactAddress = updatedForm.ContactAddress;
                savedForm.LivingConditions = updatedForm.LivingConditions;
                savedForm.HasOtherPets = updatedForm.HasOtherPets;
                savedForm.AvailableContactTime = updatedForm.AvailableContactTime;
                savedForm.UpdatedAt = dateTimeProvider.UtcNow;
                unitOfWork.GenericRepository<AdoptionForm>().Update(savedForm);
                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi cập nhật đơn nhận nuôi.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<Pagination<OrganFormBasicDto>> GetAdoptFormsForOrganizationAsync(
            FormListRequestDto requestDto, string organizationId)
        {
            var queryStatus = requestDto.FilterByStatus;
            if (queryStatus is null or FormStatus.Pending or FormStatus.UnderReview)
                await UpdateNoLongerEditableForms(organizationId);

            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            Expression<Func<AdoptionForm, bool>> filter = f
                => f.Pet.OrganizationId.Equals(organizationId)
                   && (requestDto.FilterByStatus == null ||
                       f.Status.Equals(queryStatus));
            var sort = requestDto.Sort;
            return await unitOfWork.GenericRepository<AdoptionForm>()
                .GetPaginationAsync<OrganFormBasicDto>(pageIndex, pageSize, null, filter, sort);
        }

        public async Task<OneOf<bool, IError>> ApproveAdoptionFormAsync(int formId, string organizationId)
        {
            var form = await unitOfWork.GenericRepository<AdoptionForm>()
                .GetAsync(f => f.Id == formId && f.Pet.OrganizationId.Equals(organizationId));
            if (form == null) return new NotFoundError();

            try
            {
                form.Status = FormStatus.Approved;
                unitOfWork.GenericRepository<AdoptionForm>().Update(form);

                var newInterview = new InterviewSchedule
                {
                    AdoptionFormId = formId,
                    InterviewDate = form.AvailableContactTime,
                    Status = InterviewScheduleStatus.Scheduled,
                    CreatedAt = dateTimeProvider.UtcNow,
                    UpdatedAt = dateTimeProvider.UtcNow
                };
                await unitOfWork.GenericRepository<InterviewSchedule>().InsertAsync(newInterview);
                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi phê duyệt đơn nhận nuôi.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<bool> RejectAdoptionFormAsync(int formId, string organizationId, string rejectReason)
        {
            var result = await unitOfWork.GenericRepository<AdoptionForm>()
                .ExecuteUpdateAsync(f => f.Id == formId && f.Pet.OrganizationId.Equals(organizationId),
                    update => update
                        .SetProperty(f => f.Status, FormStatus.Rejected)
                        .SetProperty(f => f.RejectReason, rejectReason));
            return result > 0;
        }

        private Task<int> UpdateNoLongerEditableForms(string requesterId)
        {
            Expression<Func<AdoptionForm, bool>> filter = form
                => (form.FormSubmitterId.Equals(requesterId) || form.Pet.OrganizationId.Equals(requesterId))
                   && form.Status == FormStatus.Pending
                   && form.CreatedAt <= dateTimeProvider.UtcNow.AddHours(-24);
            return unitOfWork.GenericRepository<AdoptionForm>().ExecuteUpdateAsync(filter,
                updates => updates
                    .SetProperty(f => f.Status, FormStatus.UnderReview)
                    .SetProperty(f => f.UpdatedAt, dateTimeProvider.UtcNow)
            );
        }
    }
}