using System.ComponentModel.DataAnnotations;

namespace TalkingTails.API.Models.Organizations
{
    public class UpdateRequest
    {
        [Required(ErrorMessage = "Id là bắt buộc")]
        public required string Id { get; set; }

        public IFormFile? ProfileImage { get; set; }

        [Required(ErrorMessage = "Tên tổ chức là bắt buộc")]
        public required string Name { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public required string Email { get; set; }

        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public required string PhoneNumber { get; set; }

        [Required(ErrorMessage = "Địa chỉ là bắt buộc")]
        public required string Address { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        public required string Description { get; set; }

        public string? MeetLink { get; set; }
    }
}