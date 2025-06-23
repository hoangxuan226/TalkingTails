using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.Donations;
using TalkingTails.API.Models.Users;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController(
        IUserService userService,
        IAdoptedPetService adoptedPetService,
        IDonationService donationService,
        IOrganizationService organizationService) : ApiController
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
        [HttpPut]
        [Route("/api/customers/me")]
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

        /// <summary>
        ///     Organization: Update organization profile
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Route("/api/organization/me")]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> UpdateOrganizationProfileAsync([FromForm] EditOrganizationRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await organizationService.UpdateAsync(request.ToOrganUpdateRequestDto(userId));
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Hồ sơ được cập nhật thành công." }),
                Problem
            );
        }

        /// <summary>
        ///     Admin: Get all customers for admin
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/customers")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetAllCustomersAsync([FromQuery] AdminUserListRequest request)
        {
            var requestDto = request.ToListRequestDto();
            var customers = await userService.GetAllForAdminAsync(requestDto);
            return Ok(customers);
        }

        /// <summary>
        ///     Admin: Get customer details by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/customers/{id}")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetCustomerDetailsAsync(string id)
        {
            var customerDetails = await userService.GetCustomerDetailsAsync(id);
            return customerDetails != null ? Ok(customerDetails) : Problem(new NotFoundError());
        }

        /// <summary>
        ///     Admin: Get adopted pets by customer
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/customers/{id}/adopted-pets")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetAdoptedPetsByCustomerAsync(string id)
        {
            var adoptedPets = await adoptedPetService.GetAdoptedPetsByUserAsync(id);
            return Ok(adoptedPets);
        }

        /// <summary>
        ///     Admin: Get donations by customer
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/customers/{id}/donations")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetDonationsByCustomerAsync(string id,
            [FromQuery] CustomerDonationListRequest request)
        {
            var requestDto = request.ToCustomerDonationListRequestDto(id);
            var donations = await donationService.GetCustomerDonationHistoryAsync(requestDto);
            return Ok(donations);
        }
    }
}