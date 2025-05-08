using System.ComponentModel.DataAnnotations;
using TalkingTails.API.Helpers.ValidationAttributes;

namespace TalkingTails.API.Models.Authentication
{
    public class RegisterRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }

        [PasswordValidation]
        public required string Password { get; set; }
    }
}
