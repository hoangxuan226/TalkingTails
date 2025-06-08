namespace TalkingTails.Business.Models.Setting
{
    public class PayOsSettings
    {
        public const string SectionName = "PayOs";
        public string ClientId { get; init; } = null!;
        public string ApiKey { get; init; } = null!;

        public string ChecksumKey { get; init; } = null!;
        //public int ExpireInMinutes { get; init; }
    }
}