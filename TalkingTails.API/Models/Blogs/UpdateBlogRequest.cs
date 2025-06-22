using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.Blogs
{
    public class UpdateBlogRequest
    {
        [Required(ErrorMessage = "ID là bắt buộc")]
        public int Id { get; set; }

        public IFormFile? CoverImage { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        public required string Title { get; set; }

        public IFormFile? ContentFile { get; set; }

        [Required(ErrorMessage = "Mô tả ngắn là bắt buộc")]
        public required string ShortContent { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PetSpecies Species { get; set; }

        [Required(ErrorMessage = "Tên tác giả là bắt buộc")]
        public required string AuthorName { get; set; }
    }
}