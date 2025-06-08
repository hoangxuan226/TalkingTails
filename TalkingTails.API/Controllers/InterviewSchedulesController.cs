using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.InterviewSchedules;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class InterviewSchedulesController(IInterviewScheduleService interviewScheduleService) : ApiController
    {
        /// <summary>
        ///     Organization: Gets list of interview schedules for the organization.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> GetInterviewSchedulesAsync([FromQuery] InterviewListRequest request)
        {
            var requestDto = request.ToInterviewListRequestDto();
            var interviews = await interviewScheduleService.GetOrganInterviewSchedulesAsync(requestDto);
            return Ok(interviews);
        }

        /// <summary>
        ///     Organization: Updates the interview schedule by the organization.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> UpdateInterviewScheduleAsync([FromBody] UpdateInterviewRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var requestDto = request.ToUpdateInterviewRequestDto(userId);
            var result = await interviewScheduleService.UpdateInterviewScheduleAsync(requestDto);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Lịch phỏng vấn đã được cập nhật." }),
                Problem
            );
        }
    }
}