using TalkingTails.Repository.Constants;

namespace TalkingTails.Repository.Entities
{
    public class Donation
    {
        public int Id { get; set; }
        public string UserId { get; set; }
        public double Amount { get; set; }
        public int DonationPackageId { get; set; }
        public string Message { get; set; } = string.Empty;
        public TransactionStatus TransactionStatus { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public ApplicationUser User { get; set; }
        public DonationPackage DonationPackage { get; set; }
    }
}