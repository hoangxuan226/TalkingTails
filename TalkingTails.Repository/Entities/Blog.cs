using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Repository.Entities
{
    public class Blog
    {
        public int Id { get; set; }
        public string CoverImageUrl { get; set; }
        [VieSearchable] public string Title { get; set; }
        [VieSearchable] public string Content { get; set; }
        public PetSpecies Species { get; set; }
        public string CreatorId { get; set; }
        public string Slug { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public BlogStatus Status { get; set; }
        public int ViewCount { get; set; }
        public ApplicationUser Creator { get; set; }
    }
}