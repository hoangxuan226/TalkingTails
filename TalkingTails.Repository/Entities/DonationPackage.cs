using TalkingTails.Repository.Constants;

namespace TalkingTails.Repository.Entities
{
    public class DonationPackage
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; } = string.Empty;
        public double Amount { get; set; }
        public DonationPackageStatus Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ICollection<Donation> Donations { get; set; }
    }
}