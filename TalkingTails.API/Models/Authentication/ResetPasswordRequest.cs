using System.ComponentModel.DataAnnotations;
using TalkingTails.API.Helpers.ValidationAttributes;

namespace TalkingTails.API.Models.Authentication
{
    public class ResetPasswordRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Token là bắt buộc")]
        public required string Token { get; set; }

        [PasswordValidation] public required string NewPassword { get; set; }
    }
}