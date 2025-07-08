namespace TalkingTails.Business.Models.Setting
{
    public class EmailSettings
    {
        public const string SectionName = "EmailSettings";
        public string SmtpHost { get; init; } = string.Empty;
        public int SmtpPort { get; init; }
        public string Username { get; init; } = string.Empty;
        public string Password { get; init; } = string.Empty;
        public bool EnableSsl { get; init; } = true;
        public string FromEmail { get; init; } = string.Empty;
        public string FromName { get; init; } = string.Empty;
        public string ClientUrl { get; init; } = string.Empty;
    }
}