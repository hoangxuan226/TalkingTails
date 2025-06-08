using System.Text.RegularExpressions;

namespace TalkingTails.Repository.Helpers
{
    public static class StringExtensions
    {
        private static readonly string[] VietnameseSigns =
        [
            "aAeEoOuUiIdDyY",
            "áàạảãâấầậẩẫăắằặẳẵ",
            "ÁÀẠẢÃÂẤẦẬẨẪĂẮẰẶẲẴ",
            "éèẹẻẽêếềệểễ",
            "ÉÈẸẺẼÊẾỀỆỂỄ",
            "óòọỏõôốồộổỗơớờợởỡ",
            "ÓÒỌỎÕÔỐỒỘỔỖƠỚỜỢỞỠ",
            "úùụủũưứừựửữ",
            "ÚÙỤỦŨƯỨỪỰỬỮ",
            "íìịỉĩ",
            "ÍÌỊỈĨ",
            "đ",
            "Đ",
            "ýỳỵỷỹ",
            "ÝỲỴỶỸ"
        ];

        public static string RemoveAccents(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            for (int i = 1; i < VietnameseSigns.Length; i++)
            {
                for (int j = 0; j < VietnameseSigns[i].Length; j++)
                    text = text.Replace(VietnameseSigns[i][j], VietnameseSigns[0][i - 1]);
            }

            return text;
        }

        public static string Standardizing(this string text)
        {
            return string.IsNullOrEmpty(text) ? string.Empty : text.RemoveAccents().ToLower();
        }

        public static string ToSlug(this string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return string.Empty;
            }

            // Remove accents and standardize the text
            text = text.RemoveAccents().ToLower();

            // Replace spaces and invalid characters with hyphens
            text = Regex.Replace(text, @"\s+", "-"); // Replace spaces with hyphens
            text = Regex.Replace(text, @"[^a-z0-9\-]", ""); // Remove invalid characters

            // Trim hyphens from the start and end
            text = text.Trim('-');

            return text;
        }
    }
}