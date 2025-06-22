using TalkingTails.API.Models.BlogComments;
using TalkingTails.API.Models.Blogs;
using TalkingTails.Business.Models.BlogComments;
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
                Sort = request.Sort
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

        public static BlogCommentListRequestDto ToBlogCommentListRequestDto(this BlogCommentListRequest request,
            int blogId)
        {
            return new BlogCommentListRequestDto
            {
                BlogId = blogId,
                PageIndex = request.PageIndex,
                PageSize = request.PageSize
            };
        }

        public static CommentRequestDto ToCommentRequestDto(this CommentRequest request, int blogId, string authorId)
        {
            return new CommentRequestDto
            {
                Content = request.Content,
                AuthorId = authorId,
                BlogId = blogId
            };
        }
    }
}