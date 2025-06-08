using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.InterviewSchedules;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IInterviewScheduleService
    {
        Task<Pagination<OrganInterviewBasicDto>> GetOrganInterviewSchedulesAsync(InterviewListRequestDto requestDto);
        Task<OneOf<bool, IError>> UpdateInterviewScheduleAsync(UpdateInterviewRequestDto requestDto);
    }
}