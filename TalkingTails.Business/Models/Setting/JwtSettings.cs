namespace TalkingTails.Business.Models.Setting
{
    public class JwtSettings
    {
        public const string SectionName = "JwtSettings";
        public string Key { get; init; } = null!;
        public int AccessTokenExpireInMinutes { get; init; }
        public int RefreshTokenExpireInMinutes { get; init; }
        public string Issuer { get; init; } = null!;
        public string Audience { get; init; } = null!;
    }
}
