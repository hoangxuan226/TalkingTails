namespace TalkingTails.Repository.Entities
{
    public class DonationLinkRequest
    {
        public int Id { get; set; }
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; } = string.Empty;
        public string ReturnUrl { get; set; } = string.Empty;
        public string CancelUrl { get; set; } = string.Empty;
        public string CheckoutUrl { get; set; } = string.Empty;
        public string UserId { get; set; }
        public string OrganizationId { get; set; }
        public string PackageName { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime UpdatedAt { get; set; }
        public string Status { get; set; } = string.Empty;
        public Donation? Donation { get; set; }
    }
}