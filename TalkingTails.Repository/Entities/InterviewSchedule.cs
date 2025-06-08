using TalkingTails.Repository.Constants;

namespace TalkingTails.Repository.Entities
{
    public class InterviewSchedule
    {
        public int Id { get; set; }
        public int AdoptionFormId { get; set; }
        public DateTime InterviewDate { get; set; }
        public InterviewScheduleStatus Status { get; set; }
        public string Notes { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public AdoptionForm AdoptionForm { get; set; }
    }
}