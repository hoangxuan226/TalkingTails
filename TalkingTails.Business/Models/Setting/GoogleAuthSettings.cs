namespace TalkingTails.Business.Models.Setting
{
    public class GoogleAuthSettings
    {
        public const string SectionName = "GoogleAuth";
        public string ClientId { get; init; } = string.Empty;
        public string ClientSecret { get; init; } = string.Empty;
        public string TokenInfoUrl { get; init; } = string.Empty;
        public string UserInfoUrl { get; init; } = string.Empty;
    }
}