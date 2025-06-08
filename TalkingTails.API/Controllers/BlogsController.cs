using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.Blogs;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BlogsController(IBlogService blogService) : ApiController
    {
        /// <summary>
        ///     Get blog list
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetNewestAvailable([FromQuery] BlogListRequest request)
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
    }
}