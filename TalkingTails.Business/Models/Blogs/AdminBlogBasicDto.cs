using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Blogs
{
    public class AdminBlogBasicDto
    {
        public required int Id { get; set; }
        public required string Title { get; set; }
        public required string AuthorName { get; set; }
        public required BlogStatus Status { get; set; }
        public required int ViewCount { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}