namespace TalkingTails.Business.Models.Donations
{
    public class TopDonorsDto
    {
        public required string Name { get; set; }
        public string? ProfileImage { get; set; }
        public long TotalDonatedAmount { get; set; }
    }
}