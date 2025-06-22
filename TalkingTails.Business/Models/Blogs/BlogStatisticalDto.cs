using TalkingTails.Repository.Constants;

namespace TalkingTails.Business.Models.Blogs
{
    public class BlogStatisticalDto
    {
        public int TotalBlogs { get; set; }
        public int TotalViews { get; set; }
        public PetSpecies? MostViewedSpecies { get; set; }
        public int MostViewedSpeciesViewCount { get; set; }
    }
}