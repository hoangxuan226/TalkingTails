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
    }
}