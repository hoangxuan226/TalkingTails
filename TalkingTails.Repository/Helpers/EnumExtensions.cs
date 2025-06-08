namespace TalkingTails.Repository.Helpers
{
    public static class EnumExtensions
    {
        public static T? ToEnum<T>(this string value) where T : struct, Enum
        {
            return Enum.TryParse<T>(value, out var result) ? result : null;
        }
    }
}