using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.InterviewSchedules
{
    public class UpdateInterviewRequestDto
    {
        public int Id { get; set; }
        public required string OrganizationId { get; set; }
        public required InterviewScheduleStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}