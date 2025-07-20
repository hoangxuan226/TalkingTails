namespace TalkingTails.Business.Models.Donations
{
    public class CreateCheckoutRequestDto
    {
        public string? DonorId { get; set; }
        public required string OrganizationId { get; set; }
        public required int DonationPackageId { get; set; }
        public required string ReturnUrl { get; set; }
        public required string CancelUrl { get; set; }
        public required string Message { get; set; }
    }
}