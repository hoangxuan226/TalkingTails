using System.ComponentModel.DataAnnotations;

namespace TalkingTails.API.Models.AdoptionForms
{
    public class CreateAdoptFormRequest
    {
        [Required] public required int PetId { get; set; }

        [Required(ErrorMessage = "Họ tên là bắt buộc")]
        [RegularExpression(@"^[^\d]+$", ErrorMessage = "Họ tên không được chứa số.")]
        [StringLength(100, ErrorMessage = "Họ tên không được vượt quá 100 ký tự.")]
        public required string FullName { get; set; }

        /// <summary>
        ///     Start with 0 or +84, followed by 9 digits.
        /// </summary>
        [Required(ErrorMessage = "Số điện thoại là bắt buộc")]
        [RegularExpression(@"^(0|\+84)[0-9]{9}$", ErrorMessage = "Số điện thoại không hợp lệ.")]
        public required string ContactPhone { get; set; }

        [Required(ErrorMessage = "Email là bắt buộc")]
        [EmailAddress(ErrorMessage = "Địa chỉ email không hợp lệ.")]
        public required string ContactEmail { get; set; }

        [Required(ErrorMessage = "Địa chỉ liên hệ là bắt buộc")]
        public required string ContactAddress { get; set; }

        [Required(ErrorMessage = "Điều kiện sống là bắt buộc")]
        public required string LivingConditions { get; set; }

        [Required] public required bool HasOtherPets { get; set; }

        [Required(ErrorMessage = "Thời gian liên hệ là bắt buộc")]
        public required DateTime AvailableContactTime { get; set; }
    }
}