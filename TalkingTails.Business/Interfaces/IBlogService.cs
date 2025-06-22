using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.Blogs;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IBlogService
    {
        Task<Pagination<GuestBlogBasicDto>> GetBlogsForGuestAsync(BlogListRequestDto requestDto);
        Task<Pagination<AdminBlogBasicDto>> GetBlogsForAdminAsync(BlogListRequestDto requestDto);
        Task<GuestBlogDetailDto?> GetBlogDetailsForGuestAsync(string slug);
        Task<AdminBlogDetailDto?> GetBlogDetailsForAdminAsync(int id);
        Task<bool> PlusViewCount(int blogId, int increment);
        Task<BlogStatisticalDto> GetBlogStatisticsAsync(DateTime? startDate, DateTime? endDate);
        Task<OneOf<bool, IError>> CreateBlogAsync(CreateBlogRequestDto requestDto);
        Task<OneOf<bool, IError>> UpdateBlogAsync(UpdateBlogRequestDto requestDto);
        Task<bool> UpdateBlogStatus(int id, BlogStatus status);
    }
}