using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.Users;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(IUserService userService) : ApiController
    {
        /// <summary>
        ///     Get profile details for the authenticated user
        /// </summary>
        /// <returns></returns>
        [HttpGet("me")]
        [Authorize]
        public async Task<IActionResult> GetProfileAsync()
        {
            var account = await userService.GetAccountDetailsAsync(User);

            return account != null ? Ok(account) : Problem(new NotFoundError());
        }

        /// <summary>
        ///     Customer: Update customer profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut("customers")]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> UpdateCustomerProfileAsync([FromBody] EditCustomerRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await userService.UpdateAsync(request.ToEditCustomerRequestDto(userId));
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Hồ sơ được cập nhật thành công." }),
                Problem
            );
        }
    }
}