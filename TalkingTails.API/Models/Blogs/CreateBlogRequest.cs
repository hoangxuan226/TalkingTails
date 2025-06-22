using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.Blogs
{
    public class CreateBlogRequest
    {
        [Required(ErrorMessage = "Trang bìa là bắt buộc")]
        public required IFormFile CoverImage { get; set; }

        [Required(ErrorMessage = "Tiêu đề là bắt buộc")]
        public required string Title { get; set; }

        [Required(ErrorMessage = "File nội dung là bắt buộc")]
        public required IFormFile ContentFile { get; set; }

        [Required(ErrorMessage = "Mô tả ngắn là bắt buộc")]
        public required string ShortContent { get; set; }

        [Required(ErrorMessage = "Danh mục là bắt buộc")]
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PetSpecies Species { get; set; }

        [Required(ErrorMessage = "Tên tác giả là bắt buộc")]
        public required string AuthorName { get; set; }
    }
}