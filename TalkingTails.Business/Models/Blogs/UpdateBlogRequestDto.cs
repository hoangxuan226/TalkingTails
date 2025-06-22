using Microsoft.AspNetCore.Http;
using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Blogs
{
    public class UpdateBlogRequestDto
    {
        public required int Id { get; set; }
        public IFormFile? CoverImage { get; set; }
        public required string Title { get; set; }
        public IFormFile? ContentFile { get; set; }
        public required string ShortContent { get; set; }
        public PetSpecies Species { get; set; }
        public required string AuthorName { get; set; }
    }
}