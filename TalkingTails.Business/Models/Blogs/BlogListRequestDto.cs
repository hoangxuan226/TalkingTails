using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Blogs
{
    public class BlogListRequestDto
    {
        public required int? PageIndex { get; set; }
        public required int? PageSize { get; set; }
        public PetSpecies? FilterBySpecies { get; set; }
        public required string? SearchByTitle { get; set; }
        public required string? Sort { get; set; }
    }
}