using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.BindingAttributes;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.Pets
{
    public class UpdatePetRequest
    {
        [Required(ErrorMessage = "Id là bắt buộc")]
        public int Id { get; set; }

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
        public required List<PetInfoUpdate> Information { get; set; }

        public List<string> ExistingImageUrls { get; set; } = [];
        public ICollection<IFormFile> NewPetImages { get; set; } = [];
    }

    [ModelBinder<DtoFormBinder>]
    public class PetInfoUpdate
    {
        public required string Label { get; set; }
        public bool Value { get; set; }
    }
}