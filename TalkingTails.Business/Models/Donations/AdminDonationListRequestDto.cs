namespace TalkingTails.Business.Models.Donations
{
    public class AdminDonationListRequestDto
    {
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
        public DateTime? FilterByStartDate { get; set; }
        public DateTime? FilterByEndDate { get; set; }
        public string? SearchByPackageName { get; set; }
        public string? Sort { get; set; }
    }
}