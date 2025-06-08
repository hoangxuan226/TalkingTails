using System.Linq.Expressions;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.InterviewSchedules;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class InterviewScheduleService(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider)
        : IInterviewScheduleService
    {
        public Task<Pagination<OrganInterviewBasicDto>> GetOrganInterviewSchedulesAsync(
            InterviewListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            Expression<Func<InterviewSchedule, bool>> filter = i =>
                (requestDto.FilterByStatus == null || i.Status.Equals(requestDto.FilterByStatus))
                && (requestDto.FilterByStartDate == null || i.InterviewDate >= requestDto.FilterByStartDate)
                && (requestDto.FilterByEndDate == null || i.InterviewDate <= requestDto.FilterByEndDate)
                && (requestDto.PetId == null || i.AdoptionForm.PetId == requestDto.PetId);
            return unitOfWork.GenericRepository<InterviewSchedule>()
                .GetPaginationAsync<OrganInterviewBasicDto>(pageIndex, pageSize, null, filter, requestDto.Sort);
        }

        public async Task<OneOf<bool, IError>> UpdateInterviewScheduleAsync(UpdateInterviewRequestDto requestDto)
        {
            var interview = await unitOfWork.GenericRepository<InterviewSchedule>()
                .GetAsync(
                    i => i.Id == requestDto.Id && i.AdoptionForm.Pet.OrganizationId.Equals(requestDto.OrganizationId),
                    nameof(InterviewSchedule.AdoptionForm));
            if (interview == null) return new NotFoundError();

            try
            {
                interview.Status = requestDto.Status;
                interview.Notes = requestDto.Notes ?? interview.Notes;
                interview.UpdatedAt = dateTimeProvider.UtcNow;
                unitOfWork.GenericRepository<InterviewSchedule>().Update(interview);

                if (!requestDto.Status.Equals(InterviewScheduleStatus.Scheduled))
                {
                    var pet = await unitOfWork.GenericRepository<Pet>()
                        .GetAsync(p => p.Id == interview.AdoptionForm.PetId);
                    if (pet == null) return new NotFoundError();

                    pet.LastInterviewed = interview.InterviewDate;
                    unitOfWork.GenericRepository<Pet>().Update(pet);
                }

                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi cập nhật lịch phỏng vấn.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }
    }
}