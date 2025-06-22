using Microsoft.AspNetCore.Http;
using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Blogs
{
    public class CreateBlogRequestDto
    {
        public required IFormFile CoverImage { get; set; }
        public required string Title { get; set; }
        public required IFormFile ContentFile { get; set; }
        public required string ShortContent { get; set; }
        public PetSpecies Species { get; set; }
        public required string AuthorName { get; set; }
    }
}