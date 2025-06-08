namespace TalkingTails.API.Models.Donations
{
    public class CreateCheckoutRequest
    {
        public required string OrganizationId { get; set; }
        public required int DonationPackageId { get; set; }
        public required string ReturnUrl { get; set; }
        public required string CancelUrl { get; set; }
        public string? Message { get; set; }
    }
}