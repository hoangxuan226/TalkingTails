using TalkingTails.Repository.Constants;

namespace TalkingTails.Repository.Entities
{
    public class CustomerDetails
    {
        public CustomerStatus Status { get; set; }
        public long TotalDonatedAmount { get; set; } = 0;
    }
}