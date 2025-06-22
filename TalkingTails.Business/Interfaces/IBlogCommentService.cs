using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.BlogComments;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IBlogCommentService
    {
        Task<Pagination<BlogCommentDto>> GetBlogCommentByBlogIdAsync(BlogCommentListRequestDto requestDto);
        Task<OneOf<bool, IError>> CommentAsync(CommentRequestDto requestDto);
    }
}