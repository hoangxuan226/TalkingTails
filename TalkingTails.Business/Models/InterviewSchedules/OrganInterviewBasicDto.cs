using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.InterviewSchedules
{
    public class OrganInterviewBasicDto : IMappable<InterviewSchedule>
    {
        public required int Id { get; set; }
        public required string CustomerName { get; set; }
        public required string PhoneNumber { get; set; }
        public required string PetName { get; set; }
        public required string MeetLink { get; set; }
        public DateTime InterviewDate { get; set; }
        public InterviewScheduleStatus Status { get; set; }
        public required string Notes { get; set; }

        public static Dictionary<string, Expression<Func<InterviewSchedule, object>>> Mappings { get; } = new()
        {
            { nameof(CustomerName), ic => ic.AdoptionForm.FullName },
            { nameof(PhoneNumber), ic => ic.AdoptionForm.ContactPhone },
            { nameof(PetName), ic => ic.AdoptionForm.Pet.PetName },
            { nameof(MeetLink), ic => ic.AdoptionForm.Pet.Organization.Organization.MeetLink }
        };
    }
}