namespace TalkingTails.Business.Models.DonationPackages
{
    public class PackageBasicDto
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; }
        public required int Amount { get; set; }
    }
}