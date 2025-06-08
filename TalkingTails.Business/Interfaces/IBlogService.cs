using TalkingTails.Business.Models.Blogs;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IBlogService
    {
        Task<Pagination<GuestBlogBasicDto>> GetBlogsForGuestAsync(BlogListRequestDto requestDto);
        Task<Pagination<AdminBlogBasicDto>> GetBlogsForAdminAsync(BlogListRequestDto requestDto);
    }
}