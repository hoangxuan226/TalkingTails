using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Blogs
{
    public class AdminBlogDetailDto
    {
        public int Id { get; set; }
        public required string CoverImageUrl { get; set; }
        public required string Title { get; set; }
        public required string ContentUrl { get; set; }
        public required string ShortContent { get; set; }
        public PetSpecies Species { get; set; }
        public required string AuthorName { get; set; }
        public required string Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public BlogStatus Status { get; set; }
        public int ViewCount { get; set; }
    }
}