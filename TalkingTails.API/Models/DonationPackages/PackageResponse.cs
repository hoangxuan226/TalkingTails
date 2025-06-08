namespace TalkingTails.API.Models.DonationPackages
{
    public class PackageResponse
    {
        public required int Id { get; set; }
        public required string Name { get; set; }
        public required string Description { get; set; } = string.Empty;
        public required int Amount { get; set; }
        public required string Status { get; set; }
        public required DateTime CreatedAt { get; set; }
        public required DateTime UpdatedAt { get; set; }
    }
}