namespace TalkingTails.Repository.Entities
{
    public class Donation
    {
        public int Id { get; set; }

        public string? UserId { get; set; } // Nullable to allow for anonymous donations
        public string OrganizationId { get; set; }
        public int Amount { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; }
        public int DonationLinkRequestId { get; set; }

        public ApplicationUser? User { get; set; }
        public ApplicationUser Organization { get; set; }
        public DonationLinkRequest DonationLinkRequest { get; set; }
    }
}