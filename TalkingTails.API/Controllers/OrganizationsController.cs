using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.Organizations;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class OrganizationsController(IUserService userService, IOrganizationService organizationService)
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

        [HttpPost]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Create([FromBody] CreateRequest request)
        {
            var organization = request.ToApplicationUser();
            var role = nameof(Roles.Organization);
            var result = await userService.CreateAsync(organization, request.Password, [role]); // temp
            return result.Match<IActionResult>(
                user => Ok(user.ToOrganizationResponse()),
                Problem
            );
        }
    }
}