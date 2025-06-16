using System.Linq.Expressions;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Organizations
{
    public class OrganizationBasicDto : IMappable<ApplicationUser>
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }

        public static Dictionary<string, Expression<Func<ApplicationUser, object>>> Mappings { get; } = new()
        {
            { nameof(Description), u => u.Organization != null ? u.Organization.Description : string.Empty }
        };
    }
}