using System.ComponentModel.DataAnnotations;

namespace TalkingTails.API.Models.Authentication
{
    public class GoogleLoginRequest
    {
        [Required(ErrorMessage = "Google token là bắt buộc")]
        public required string GoogleToken { get; set; }
    }
}