using System.ComponentModel.DataAnnotations;

namespace TalkingTails.API.Models.Users
{
    public class UpdateAvatarRequest
    {
        [Required(ErrorMessage = "Avatar là bắt buộc")]
        public required IFormFile Avatar { get; set; }
    }
}