using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Net.payOS.Types;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.Donations;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [Authorize]
    [ApiController]
    public class DonationsController(IDonationService donationService) : ApiController
    {
        /// <summary>
        ///     Creates a checkout URL (QR code) for donations.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost("create-checkout")]
        [AllowAnonymous]
        public async Task<IActionResult> CreateCheckout([FromBody] CreateCheckoutRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var checkoutUrl = await donationService.CreateCheckoutUrl(
                request.ToCreateCheckoutRequestDto(userId)
            );
            return checkoutUrl.Match<IActionResult>(
                url => Ok(new { url }),
                Problem
            );
        }

        [HttpPost("webhook/payos")]
        [AllowAnonymous]
        [ApiExplorerSettings(IgnoreApi = true)]
        public async Task<IActionResult> PayOsTransferHandler(WebhookType body)
        {
            var success = await donationService.PayOsTransferHandler(body);
            return success ? Ok() : NoContent();
        }

        /// <summary>
        ///     Gets the top donors.
        /// </summary>
        /// <param name="count"></param>
        /// <returns></returns>
        [HttpGet("top-donors")]
        [AllowAnonymous]
        public async Task<IActionResult> GetTopDonors([FromQuery] int count)
        {
            var topDonors = await donationService.GetTopDonorsAsync(count);
            return Ok(topDonors);
        }

        /// <summary>
        ///     Customer: Gets customer's donation history.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> GetCustomerDonationHistory([FromQuery] CustomerDonationListRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var requestDto = request.ToCustomerDonationListRequestDto(userId ?? string.Empty);
            var donations = await donationService.GetCustomerDonationHistoryAsync(requestDto);
            return Ok(donations);
        }

        /// <summary>
        ///     Admin: Gets the donation history for admin.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/admin/[controller]")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetAdminDonationHistory([FromQuery] AdminDonationListRequest request)
        {
            var requestDto = request.ToAdminDonationListRequestDto();
            var donations = await donationService.GetAdminDonationHistoryAsync(requestDto);
            return Ok(donations);
        }

        /// <summary>
        ///     Admin: Gets the donation statistics within a specified date range.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/admin/[controller]/statistic")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetDonationStatistics([FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var statistics = await donationService.GetDonationStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
    }
}