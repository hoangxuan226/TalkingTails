namespace TalkingTails.Repository.Entities
{
    public class OrganizationDetails
    {
        public string Description { get; set; } = string.Empty;
        public string MeetLink { get; set; } = string.Empty;
        public long TotalDonationAmount { get; set; } = 0;
    }
}