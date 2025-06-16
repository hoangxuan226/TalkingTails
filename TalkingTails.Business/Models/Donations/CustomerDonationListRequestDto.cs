namespace TalkingTails.Business.Models.Donations
{
    public class CustomerDonationListRequestDto
    {
        public required string UserId { get; set; }
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
    }
}