using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Users
{
    public class AdminUserBasicDto : IMappable<ApplicationUser>
    {
        public required string Id { get; set; }
        public required string Name { get; set; }
        public required string Email { get; set; }
        public CustomerStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }

        public static Dictionary<string, Expression<Func<ApplicationUser, object>>> Mappings { get; } = new()
        {
            { nameof(Status), u => u.Customer!.Status }
        };
    }
}