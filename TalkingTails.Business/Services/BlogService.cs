using System.Linq.Expressions;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Blogs;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class BlogService(IUnitOfWork unitOfWork) : IBlogService
    {
        public async Task<Pagination<GuestBlogBasicDto>> GetBlogsForGuestAsync(BlogListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            (string, string)? vietnameseSearch =
                requestDto.SearchByTitle == null ? null : (requestDto.SearchByTitle, nameof(Blog.Title));
            Expression<Func<Blog, bool>> filter = p =>
                (requestDto.FilterBySpecies == null || p.Species.Equals(requestDto.FilterBySpecies))
                && p.Status.Equals(BlogStatus.Active);
            return await unitOfWork.GenericRepository<Blog>()
                .GetPaginationAsync<GuestBlogBasicDto>(pageIndex, pageSize,
                    vietnameseSearch, filter);
        }

        public async Task<Pagination<AdminBlogBasicDto>> GetBlogsForAdminAsync(BlogListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            (string, string)? vietnameseSearch =
                requestDto.SearchByTitle == null ? null : (requestDto.SearchByTitle, nameof(Blog.Title));
            Expression<Func<Blog, bool>> filter = p =>
                (requestDto.FilterBySpecies == null || p.Species.Equals(requestDto.FilterBySpecies));
            return await unitOfWork.GenericRepository<Blog>()
                .GetPaginationAsync<AdminBlogBasicDto>(pageIndex, pageSize,
                    vietnameseSearch, filter, requestDto.Sort);
        }
    }
}