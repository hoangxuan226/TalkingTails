namespace TalkingTails.Business.Models.Donations
{
    public class StatisticalDto
    {
        public int PackageCount => PackageStatistics.Count;
        public int TotalDonations => PackageStatistics.Sum(x => x.Count);
        public long TotalAmount => PackageStatistics.Sum(x => x.TotalAmount);
        public List<PackageStatistics> PackageStatistics { get; set; } = [];
    }

    public class PackageStatistics
    {
        public string PackageName { get; set; } = string.Empty;
        public int Count { get; set; }
        public long TotalAmount { get; set; }
    }
}