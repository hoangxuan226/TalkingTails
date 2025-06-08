namespace TalkingTails.Business.Interfaces
{
    public interface IDateTimeProvider
    {
        DateTime UtcNow { get; }
        DateTimeOffset UtcNowOffset { get; }
    }
}