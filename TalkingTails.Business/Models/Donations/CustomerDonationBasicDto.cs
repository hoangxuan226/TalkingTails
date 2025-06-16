using System.Linq.Expressions;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Donations
{
    public class CustomerDonationBasicDto : IMappable<Donation>
    {
        public int Id { get; set; }
        public required string PackageName { get; set; }
        public int Amount { get; set; }
        public DateTime CreatedAt { get; set; }
        public required string OrganizationName { get; set; }
        public required string Message { get; set; }

        public static Dictionary<string, Expression<Func<Donation, object>>> Mappings { get; } = new()
        {
            {
                nameof(OrganizationName),
                d => d.Organization.Name ?? d.Organization.UserName ?? d.Organization.Email ?? ""
            }
        };
    }
}