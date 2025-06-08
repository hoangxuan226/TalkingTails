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
    public class OrganizationsController(IUserService userService) : ApiController
    {
        [HttpPost]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> Create([FromBody] CreateRequest request)
        {
            var organization = request.ToApplicationUser();
            var role = nameof(Roles.Organization);
            var result = await userService.CreateAsync(organization, request.Password, [role]);
            return result.Match<IActionResult>(
                user => Ok(user.ToOrganizationResponse()),
                Problem
            );
        }
    }
}