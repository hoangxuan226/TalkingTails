using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.Organizations;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrganizationsController(IOrganizationService organizationService)
        : ApiController
    {
        /// <summary>
        ///     Gets all organizations.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> GetAllAsync()
        {
            var organizations = await organizationService.GetAllAsync();
            return Ok(organizations);
        }

        /// <summary>
        ///     Admin: Creates a new organization.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("/api/admin/[controller]")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Create([FromForm] CreateRequest request)
        {
            var requestDto = request.ToCreateRequestDto();
            var result = await organizationService.CreateAsync(requestDto);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Tổ chức được tạo thành công." }),
                Problem
            );
        }

        /// <summary>
        ///     Admin: Updates an organization.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("/api/admin/[controller]")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Update([FromForm] UpdateRequest request)
        {
            var requestDto = request.ToUpdateRequestDto();
            var result = await organizationService.UpdateAsync(requestDto);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Tổ chức được cập nhật thành công." }),
                Problem
            );
        }

        /// <summary>
        ///     Admin: Gets all organizations for admin.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/admin/[controller]")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetAllForAdminAsync([FromQuery] AdminOrganListRequest request)
        {
            var requestDto = request.ToAdminOrganListRequestDto();
            var result = await organizationService.GetAllForAdminAsync(requestDto);
            return Ok(result);
        }

        /// <summary>
        ///     Admin: Get details of an organization by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/admin/[controller]/{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetDetailsAsync(string id)
        {
            var organization = await organizationService.GetDetailsAsync(id);
            return organization != null ? Ok(organization) : Problem(new NotFoundError());
        }

        /// <summary>
        ///     Admin: Count organizations group by status.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/admin/[controller]/count")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetOrganizationCountAsync()
        {
            var count = await organizationService.GetOrganizationCountAsync();
            return Ok(count);
        }
    }
}