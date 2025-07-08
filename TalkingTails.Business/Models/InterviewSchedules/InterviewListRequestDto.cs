using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.InterviewSchedules
{
    public class InterviewListRequestDto
    {
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
        public DateTime? FilterByStartDate { get; set; }
        public DateTime? FilterByEndDate { get; set; }
        public InterviewScheduleStatus? FilterByStatus { get; set; }
        public string? Sort { get; set; }
        public int? PetId { get; set; }
        public required string OrganizationId { get; set; }
    }
}