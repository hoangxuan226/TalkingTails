using System.Linq.Expressions;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Models.Users
{
    public class CustomerDetailsDto : IMappable<ApplicationUser>
    {
        public required string Name { get; set; }
        public required string Address { get; set; }
        public DateTime? Birthday { get; set; }
        public required string PhoneNumber { get; set; }
        public required string Email { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public CustomerStatus Status { get; set; }
        public required string ProfileImage { get; set; }
        public long TotalDonatedAmount { get; set; }
        public int TotalAdoptedPets { get; set; }
        public int DonationCount { get; set; }

        public static Dictionary<string, Expression<Func<ApplicationUser, object>>> Mappings { get; } = new()
        {
            { nameof(Status), user => user.Customer!.Status },
            { nameof(TotalDonatedAmount), user => user.Customer!.TotalDonatedAmount },
            { nameof(TotalAdoptedPets), user => user.AdoptedPets.Count },
            { nameof(DonationCount), user => user.Donations.Count }
        };
    }
}