using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.Pets;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PetsController(IPetService petService) : ApiController
    {
        /// <summary>
        ///     Get pet list for guest
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        public async Task<IActionResult> GetAllAsync([FromQuery] PetListRequest request)
        {
            var requestDto = request.ToListRequestDto();
            var pets = await petService.GetPetsForGuestAsync(requestDto);
            return Ok(pets);
        }

        /// <summary>
        ///     Organization: Get pet list for organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/organization/[controller]")]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> GetAllOfOrganizationAsync([FromQuery] OrganPetListRequest request)
        {
            var requestDto = request.ToListRequestDto();
            var organizationId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            var pets =
                await petService.GetPetsOfOrganizationAsync(requestDto, organizationId ?? string.Empty);
            return Ok(pets);
        }

        /// <summary>
        ///     Get pet details by slug for guest
        /// </summary>
        /// <param name="slug"></param>
        /// <returns></returns>
        [HttpGet("slug")]
        public async Task<IActionResult> GetBySlugAsync(string slug)
        {
            var petDetails = await petService.GetPetDetailsForGuestAsync(slug);
            return petDetails == null ? Problem(new NotFoundError()) : Ok(petDetails);
        }

        /// <summary>
        ///     Organization: Get pet details by id for organization
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> GetByIdAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var petDetails = await petService.GetPetDetailsForOrganizationAsync(id, userId);
            return petDetails == null ? Problem(new NotFoundError()) : Ok(petDetails);
        }

        /// <summary>
        ///     Organization: Get interviewed (but not yet adopted) pet list for organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet("interviewed")]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> GetInterviewedPetsAsync([FromQuery] InterviewedPetListRequest request)
        {
            var requestDto = request.ToInterviewedPetListRequestDto();
            var pets = await petService.GetPetWithRecentInterviewAsync(requestDto);
            return Ok(pets);
        }
    }
}