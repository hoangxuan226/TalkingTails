namespace TalkingTails.Business.Models.Blogs
{
    public class GuestBlogBasicDto
    {
        public required int Id { get; set; }
        public required string CoverImageUrl { get; set; }
        public required string Title { get; set; }
        public required string Slug { get; set; }
        public required string ContentUrl { get; set; }
        public required DateTime CreatedAt { get; set; }
    }
}