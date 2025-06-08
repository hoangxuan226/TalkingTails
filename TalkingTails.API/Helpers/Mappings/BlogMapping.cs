using TalkingTails.API.Models.Blogs;
using TalkingTails.Business.Models.Blogs;

namespace TalkingTails.API.Helpers.Mappings
{
    public static class BlogMapping
    {
        public static BlogListRequestDto ToBlogListRequestDto(this BlogListRequest request)
        {
            return new BlogListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                FilterBySpecies = request.FilterBySpecies,
                SearchByTitle = request.SearchByTitle,
                Sort = null
            };
        }

        public static BlogListRequestDto ToBlogListRequestDto(this AdminBlogListRequest request)
        {
            return new BlogListRequestDto
            {
                PageIndex = request.PageIndex,
                PageSize = request.PageSize,
                FilterBySpecies = request.FilterBySpecies,
                SearchByTitle = request.SearchByTitle,
                Sort = request.Sort
            };
        }
    }
}