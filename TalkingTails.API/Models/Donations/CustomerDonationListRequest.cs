using System.ComponentModel.DataAnnotations;

namespace TalkingTails.API.Models.Donations
{
    public class CustomerDonationListRequest
    {
        [Range(1, int.MaxValue)] public int? PageIndex { get; set; }
        [Range(1, 50)] public int? PageSize { get; set; }
    }
}