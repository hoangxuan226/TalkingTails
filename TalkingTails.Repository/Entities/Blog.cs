using TalkingTails.Repository.Constants;

namespace TalkingTails.Repository.Entities
{
    public class Blog
    {
        public int Id { get; set; }
        public string CoverImageUrl { get; set; }
        public string Title { get; set; }
        public string ContentUrl { get; set; }
        public string ShortContent { get; set; }
        public PetSpecies Species { get; set; }
        public string AuthorName { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public BlogStatus Status { get; set; }
        public int ViewCount { get; set; }
    }
}