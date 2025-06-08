using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.InterviewSchedules
{
    public class InterviewListRequest
    {
        [Range(1, int.MaxValue)] public int? PageIndex { get; set; }
        [Range(1, 50)] public int? PageSize { get; set; }
        public DateTime? FilterByStartDate { get; set; }
        public DateTime? FilterByEndDate { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public InterviewScheduleStatus? FilterByStatus { get; set; }

        public string? Sort { get; set; }
        public int? PetId { get; set; }
    }
}