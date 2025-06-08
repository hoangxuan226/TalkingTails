using System.ComponentModel.DataAnnotations;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.InterviewSchedules
{
    public class UpdateInterviewRequest
    {
        [Required] public int Id { get; set; }
        [Required] public InterviewScheduleStatus Status { get; set; }
        public string? Notes { get; set; }
    }
}