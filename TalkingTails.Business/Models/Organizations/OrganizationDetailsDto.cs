using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Organizations
{
    public class OrganizationDetailsDto : IMappable<ApplicationUser>
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Address { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public required string ProfileImage { get; set; }
        public required string Description { get; set; }
        public required string MeetLink { get; set; }
        public OrganizationStatus Status { get; set; }
        public long TotalDonationAmountReceived { get; set; }

        public static Dictionary<string, Expression<Func<ApplicationUser, object>>> Mappings { get; } = new()
        {
            { nameof(Description), user => user.Organization!.Description },
            { nameof(MeetLink), user => user.Organization!.MeetLink },
            { nameof(TotalDonationAmountReceived), user => user.Organization!.TotalDonationAmount },
            { nameof(Status), user => user.Organization!.Status }
        };
    }
}