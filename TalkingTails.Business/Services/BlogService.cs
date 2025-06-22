using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Blogs;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class BlogService(IUnitOfWork unitOfWork, IFileService fileService, IDateTimeProvider dateTimeProvider)
        : IBlogService
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
                    vietnameseSearch, filter, requestDto.Sort);
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

        public Task<GuestBlogDetailDto?> GetBlogDetailsForGuestAsync(string slug)
        {
            return unitOfWork.GenericRepository<Blog>()
                .GetAsync<GuestBlogDetailDto>(p => p.Slug.Equals(slug) && p.Status.Equals(BlogStatus.Active));
        }

        public Task<AdminBlogDetailDto?> GetBlogDetailsForAdminAsync(int id)
        {
            return unitOfWork.GenericRepository<Blog>()
                .GetAsync<AdminBlogDetailDto>(p => p.Id == id);
        }

        public async Task<bool> PlusViewCount(int blogId, int increment)
        {
            var result = await unitOfWork.GenericRepository<Blog>().ExecuteUpdateAsync(b => b.Id == blogId,
                update => update.SetProperty(b => b.ViewCount, b => b.ViewCount + increment));
            return result > 0;
        }

        public async Task<BlogStatisticalDto> GetBlogStatisticsAsync(DateTime? startDate, DateTime? endDate)
        {
            Expression<Func<Blog, bool>> filter = b => (startDate == null || b.CreatedAt >= startDate) &&
                                                       (endDate == null || b.CreatedAt <= endDate);
            var totalBlogs = await unitOfWork.GenericRepository<Blog>().GetQueryable().CountAsync(filter);
            var totalViews = await unitOfWork.GenericRepository<Blog>().GetQueryable().Where(filter)
                .SumAsync(b => b.ViewCount);
            var mostViewedSpecies = await unitOfWork.GenericRepository<Blog>()
                .GetQueryable()
                .Where(filter)
                .GroupBy(b => b.Species)
                .OrderByDescending(g => g.Sum(b => b.ViewCount))
                .Select(g => new { Species = g.Key, ViewCount = g.Sum(b => b.ViewCount) })
                .FirstOrDefaultAsync();
            return new BlogStatisticalDto
            {
                TotalBlogs = totalBlogs,
                TotalViews = totalViews,
                MostViewedSpecies = mostViewedSpecies?.Species,
                MostViewedSpeciesViewCount = mostViewedSpecies?.ViewCount ?? 0
            };
        }

        public async Task<OneOf<bool, IError>> CreateBlogAsync(CreateBlogRequestDto requestDto)
        {
            try
            {
                var coverImageUrl = await fileService.UploadAsync(requestDto.CoverImage);
                var contentUrl = await fileService.UploadAsync(requestDto.ContentFile);
                var blog = new Blog
                {
                    CoverImageUrl = coverImageUrl,
                    Title = requestDto.Title,
                    ContentUrl = contentUrl,
                    ShortContent = requestDto.ShortContent,
                    Species = requestDto.Species,
                    AuthorName = requestDto.AuthorName,
                    Slug = requestDto.Title.ToSlug(),
                    CreatedAt = dateTimeProvider.UtcNow,
                    UpdatedAt = dateTimeProvider.UtcNow,
                    Status = BlogStatus.Active,
                    ViewCount = 0
                };
                await unitOfWork.GenericRepository<Blog>().InsertAsync(blog);
                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi tạo bài viết.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<OneOf<bool, IError>> UpdateBlogAsync(UpdateBlogRequestDto requestDto)
        {
            try
            {
                var blog = await unitOfWork.GenericRepository<Blog>().GetAsync(b => b.Id == requestDto.Id);
                if (blog == null) return new NotFoundError();
                if (requestDto.CoverImage != null)
                {
                    var newCoverImageUrl = await fileService.UploadAsync(requestDto.CoverImage);
                    await fileService.DeleteAsync(blog.CoverImageUrl);
                    blog.CoverImageUrl = newCoverImageUrl;
                }

                if (requestDto.ContentFile != null)
                {
                    var newContentUrl = await fileService.UploadAsync(requestDto.ContentFile);
                    await fileService.DeleteAsync(blog.ContentUrl);
                    blog.ContentUrl = newContentUrl;
                }

                blog.Title = requestDto.Title;
                blog.ShortContent = requestDto.ShortContent;
                blog.Species = requestDto.Species;
                blog.AuthorName = requestDto.AuthorName;
                blog.Slug = requestDto.Title.ToSlug();
                blog.UpdatedAt = dateTimeProvider.UtcNow;
                unitOfWork.GenericRepository<Blog>().Update(blog);
                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi cập nhật bài viết.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<bool> UpdateBlogStatus(int id, BlogStatus status)
        {
            var result = await unitOfWork.GenericRepository<Blog>().ExecuteUpdateAsync(b => b.Id == id,
                update => update.SetProperty(b => b.Status, status));
            return result > 0;
        }
    }
}