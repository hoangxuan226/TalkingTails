using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DashboardController(IDashboardService dashboardService) : ApiController
    {
        /// <summary>
        ///     Admin: Gets the dashboard statistics for the admin.
        /// </summary>
        /// <param name="startDate"></param>
        /// <param name="endDate"></param>
        /// <returns></returns>
        [HttpGet("statistics")]
        [Authorize(Roles = nameof(Roles.Admin))]
        public async Task<IActionResult> GetStatisticsAsync([FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var statistics = await dashboardService.GetStatisticsAsync(startDate, endDate);
            return Ok(statistics);
        }
    }
}