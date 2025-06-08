using System.ComponentModel.DataAnnotations;
using TalkingTails.API.Helpers.ValidationAttributes;

namespace TalkingTails.API.Models.Organizations
{
    public class CreateRequest
    {
        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }

        [PasswordValidation] public required string Password { get; set; }
    }
}