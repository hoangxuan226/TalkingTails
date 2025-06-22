using System.ComponentModel.DataAnnotations;

namespace TalkingTails.API.Models.Users
{
    public class EditCustomerRequest
    {
        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Họ tên không được chứa số.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public required string Name { get; set; }

        public string? Address { get; set; }
        public DateTime? Birthday { get; set; }

        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public string? PhoneNumber { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public required string Email { get; set; }
    }
}