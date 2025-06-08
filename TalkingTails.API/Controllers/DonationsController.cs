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
        [HttpPost("create-checkout")]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> CreateCheckout([FromBody] CreateCheckoutRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var checkoutUrl = await donationService.CreateCheckoutUrl(
                request.ToCreateCheckoutRequestDto(userId ?? string.Empty)
            );
            return checkoutUrl.Match<IActionResult>(
                url => Ok(new { url }),
                Problem
            );
        }

        [HttpPost("webhook/payos")]
        [AllowAnonymous]
        public async Task<IActionResult> PayOsTransferHandler(WebhookType body)
        {
            var success = await donationService.PayOsTransferHandler(body);
            return success ? Ok() : NoContent();
        }
    }
}