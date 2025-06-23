using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;

namespace TalkingTails.API.Models.Pets
{
    public class CreatePetRequest
    {
        [Required(ErrorMessage = "Tên thú cưng là bắt buộc")]
        public required string PetName { get; set; }

        [Required(ErrorMessage = "Loài thú cưng là bắt buộc")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PetSpecies Species { get; set; }

        [Required(ErrorMessage = "Giống thú cưng là bắt buộc")]
        public required string Breed { get; set; }

        [Required(ErrorMessage = "Tuổi thú cưng là bắt buộc")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PetAgeCategories Age { get; set; }

        [Required(ErrorMessage = "Cân nặng là bắt buộc")]
        [Range(0.1, 100, ErrorMessage = "Cân nặng phải từ 0.1 đến 100 kg")]
        public double Weight { get; set; }

        [Required(ErrorMessage = "Giới tính là bắt buộc")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public Gender Gender { get; set; }

        [Required(ErrorMessage = "Mô tả là bắt buộc")]
        public required string Description { get; set; }

        [Required(ErrorMessage = "Nhu cầu môi trường sống là bắt buộc")]
        public required string[] LivingEnvironmentNeeds { get; set; }

        [Required(ErrorMessage = "Thông tin đặc điểm là bắt buộc")]
        public required List<PetInfoItem> Information { get; set; }

        [Required(ErrorMessage = "Hình ảnh thú cưng là bắt buộc")]
        public required ICollection<IFormFile> PetImages { get; set; }
    }
}