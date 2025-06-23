namespace TalkingTails.Business.Models.Organizations
{
    public class OrganizationCountDto
    {
        public int OrganizationCount => StatusCount.Sum(x => x.Count);
        public List<StatusCount> StatusCount { get; set; } = [];
    }

    public class StatusCount
    {
        public string StatusName { get; set; } = string.Empty;
        public int Count { get; set; }
    }
}