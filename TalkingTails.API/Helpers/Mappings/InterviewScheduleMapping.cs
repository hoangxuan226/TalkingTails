using TalkingTails.API.Models.InterviewSchedules;
using TalkingTails.Business.Models.InterviewSchedules;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class InterviewScheduleMapping
    {
        public static InterviewListRequestDto ToInterviewListRequestDto(this InterviewListRequest requestDto)
        {
            return new InterviewListRequestDto
            {
                PageIndex = requestDto.PageIndex,
                PageSize = requestDto.PageSize,
                FilterByStartDate = requestDto.FilterByStartDate,
                FilterByEndDate = requestDto.FilterByEndDate,
                FilterByStatus = requestDto.FilterByStatus,
                Sort = requestDto.Sort,
                PetId = requestDto.PetId
            };
        }

        public static UpdateInterviewRequestDto ToUpdateInterviewRequestDto(this UpdateInterviewRequest request,
            string organizationId)
        {
            return new UpdateInterviewRequestDto
            {
                Id = request.Id,
                OrganizationId = organizationId,
                Status = request.Status,
                Notes = request.Notes
            };
        }
    }
}