using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.BlogComments;
using TalkingTails.API.Models.Blogs;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController(IBlogService blogService, IBlogCommentService blogCommentService) : ApiController
    {
        /// <summary>
        ///     Get blog list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllForGuestAsync([FromQuery] BlogListRequest request)
        {
            var requestDto = request.ToBlogListRequestDto();
            var blogBasicDtos = await blogService.GetBlogsForGuestAsync(requestDto);
            return Ok(blogBasicDtos);
        }

        /// <summary>
        ///     Admin: Get blog list for admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/admin/[controller]")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetAllForAdminAsync([FromQuery] AdminBlogListRequest request)
        {
            var requestDto = request.ToBlogListRequestDto();
            var blogAdminDtos = await blogService.GetBlogsForAdminAsync(requestDto);
            return Ok(blogAdminDtos);
        }

        /// <summary>
        ///     Get blog details by slug for guest
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpGet("slug")]
        public async Task<IActionResult> GetBySlugAsync(string slug)
        {
            var blogDetails = await blogService.GetBlogDetailsForGuestAsync(slug);
            return blogDetails == null ? Problem(new NotFoundError()) : Ok(blogDetails);
        }

        /// <summary>
        ///     Admin: Get blog details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id:int}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var blogDetails = await blogService.GetBlogDetailsForAdminAsync(id);
            return blogDetails == null ? Problem(new NotFoundError()) : Ok(blogDetails);
        }

        /// <summary>
        ///     Admin: Create a new blog
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> CreateAsync(CreateBlogRequest request)
        {
            var requestDto = request.ToCreateBlogRequestDto();
            var result = await blogService.CreateBlogAsync(requestDto);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Tạo blog thành công." }),
                Problem);
        }

        /// <summary>
        ///     Admin: Update an existing blog
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> UpdateAsync(UpdateBlogRequest request)
        {
            var requestDto = request.ToUpdateBlogRequestDto();
            var result = await blogService.UpdateBlogAsync(requestDto);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Cập nhật blog thành công." }),
                Problem);
        }

        /// <summary>
        ///     Get comments by blog id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("{id:int}/comments")]
        public async Task<IActionResult> GetCommentsByBlogIdAsync(int id, [FromQuery] BlogCommentListRequest request)
        {
            var comments =
                await blogCommentService.GetBlogCommentByBlogIdAsync(request.ToBlogCommentListRequestDto(id));
            return Ok(comments);
        }

        /// <summary>
        ///     Comment blog by blog id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("{id:int}/comments")]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> CommentAsync(int id, [FromBody] CommentRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var requestDto = request.ToCommentRequestDto(id, userId ?? "");
            var result = await blogCommentService.CommentAsync(requestDto);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Comment thành công." }),
                Problem);
        }

        /// <summary>
        ///     Add view count for blog
        /// </summary>
        /// <param name="id"></param>
        /// <param name="increment"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}/add-view-count")]
        public async Task<IActionResult> PlusViewCountAsync(int id, int increment = 1)
        {
            if (increment <= 0)
            {
                return BadRequest(new { Message = "Increment must be greater than 0." });
            }

            var result = await blogService.PlusViewCount(id, increment);
            return result ? Ok(new { Message = "Cộng view count thành công." }) : Problem(new NotFoundError());
        }

        /// <summary>
        ///     Admin: Get blog statistics within a specified date range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/admin/[controller]/statistic")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetBlogStatisticsAsync([FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var statistics = await blogService.GetBlogStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }

        /// <summary>
        ///     Admin: Update blog status by id.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{id:int}/status")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> UpdateBlogStatusAsync(int id, [FromBody] UpdateBlogStatusRequest request)
        {
            var result = await blogService.UpdateBlogStatus(id, request.Status);
            return result
                ? Ok(new { Message = "Cập nhật trạng thái blog thành công." })
                : Problem(new InvalidResourcesError());
        }
    }
}