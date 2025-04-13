using TalkingTails.Business.Interfaces;

namespace TalkingTails.Business.Services
{
    public class DateTimeProvider : IDateTimeProvider
    {
        public DateTime UtcNow => DateTime.UtcNow;
    }
}
