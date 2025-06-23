using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Organizations
{
    public class AdminOrganBasicDto : IMappable<ApplicationUser>
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Address { get; set; }
        public DateTime CreatedAt { get; set; }
        public OrganizationStatus Status { get; set; }

        public static Dictionary<string, Expression<Func<ApplicationUser, object>>> Mappings { get; } = new()
        {
            { nameof(Status), o => o.Organization.Status }
        };
    }
}