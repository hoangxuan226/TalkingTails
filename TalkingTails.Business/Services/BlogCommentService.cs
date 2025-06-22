using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.BlogComments;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class BlogCommentService(IUnitOfWork unitOfWork) : IBlogCommentService
    {
        public Task<Pagination<BlogCommentDto>> GetBlogCommentByBlogIdAsync(BlogCommentListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 10;
            var sort = $"{nameof(BlogComment.CreatedAt)} desc";
            return unitOfWork.GenericRepository<BlogComment>().GetPaginationAsync<BlogCommentDto>(pageIndex, pageSize,
                null,
                b => b.BlogId.Equals(requestDto.BlogId), sort);
        }

        public async Task<OneOf<bool, IError>> CommentAsync(CommentRequestDto requestDto)
        {
            try
            {
                var exist = await unitOfWork.GenericRepository<Blog>()
                    .AnyAsync(b => b.Id.Equals(requestDto.BlogId) && b.Status.Equals(BlogStatus.Active));
                if (!exist) return new NotFoundError();
                var comment = new BlogComment
                {
                    BlogId = requestDto.BlogId,
                    AuthorId = requestDto.AuthorId,
                    Content = requestDto.Content,
                    CreatedAt = DateTime.UtcNow
                };
                await unitOfWork.GenericRepository<BlogComment>().InsertAsync(comment);
                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi nhận xét bài blog.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }
    }
}