using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;

namespace TalkingTails.API.Helpers.ValidationAttributes
{
    public class PasswordValidationAttribute : ValidationAttribute
    {
        protected override ValidationResult? IsValid(object? value, ValidationContext validationContext)
        {
            var password = value as string;

            if (string.IsNullOrEmpty(password))
            {
                return new ValidationResult("Mật khẩu là bắt buộc");
            }
            
            var errorMessages = new List<string>();

            // Check minimum length
            if (password.Length < 6)
            {
                errorMessages.Add("Mật khẩu phải có ít nhất 6 ký tự");
            }

            // Check for at least one digit
            if (!Regex.IsMatch(password, @"\d"))
            {
                errorMessages.Add("Mật khẩu phải có ít nhất một chữ số");
            }

            // Check for at least one lowercase letter
            if (!Regex.IsMatch(password, @"[a-z]"))
            {
                errorMessages.Add("Mật khẩu phải có ít nhất một chữ thường");
            }

            // Check for at least one uppercase letter
            if (!Regex.IsMatch(password, @"[A-Z]"))
            {
                errorMessages.Add("Mật khẩu phải có ít nhất một chữ cái viết hoa");
            }

            // Check for at least one non-alphanumeric character
            if (!Regex.IsMatch(password, @"[\W_]"))
            {
                errorMessages.Add("Mật khẩu phải có ít nhất một ký tự đặc biệt");
            }

            return errorMessages.Any() ? new ValidationResult(string.Join("; ", errorMessages)) : ValidationResult.Success;
        }
    }
}
