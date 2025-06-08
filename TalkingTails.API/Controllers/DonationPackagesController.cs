using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DonationPackagesController(IDonationPackageService donationPackageService) : ApiController
    {
        [HttpGet]
        public async Task<IActionResult> GetAll()
        {
            return User.IsInRole(nameof(Roles.Admin))
                ? Ok((await donationPackageService.GetAllAsync()).ToPackageResponseList())
                : Ok(await donationPackageService.GetActivePackageBasicListAsync());
        }
    }
}