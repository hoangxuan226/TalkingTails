using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.AdoptedPets;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdoptedPetsController(IAdoptedPetService adoptedPetService) : ApiController
    {
        /// <summary>
        ///     Organization: Choose pet owner
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> AdoptPetAsync([FromBody] AdoptPetRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var requestDto = request.ToAdoptPetRequestDto(userId);
            var result = await adoptedPetService.AdoptPetAsync(requestDto);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Nhận nuôi thú cưng thành công." }),
                Problem
            );
        }

        /// <summary>
        ///     Customer: Get adopted pets by customer
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> GetAdoptedPetsAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var adoptedPets = await adoptedPetService.GetAdoptedPetsByUserAsync(userId);
            return Ok(adoptedPets);
        }
    }
}