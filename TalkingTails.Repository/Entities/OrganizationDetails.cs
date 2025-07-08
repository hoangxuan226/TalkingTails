using TalkingTails.Repository.Constants;

namespace TalkingTails.Repository.Entities
{
    public class OrganizationDetails
    {
        public string Description { get; set; } = string.Empty;
        public string MeetLink { get; set; } = string.Empty;
        public long TotalDonationAmount { get; set; } = 0;

        /// <summary>
        ///     When inactive, effect:
        ///     + Get all public organization
        ///     + Get public pets belong to this organization
        /// </summary>
        public OrganizationStatus Status { get; set; }
    }
}