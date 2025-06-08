using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.AdoptionForms;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdoptionFormsController(IAdoptionFormService adoptionFormService) : ApiController
    {
        /// <summary>
        ///     Customer: Submit a new adoption form for a pet.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> SubmitAdoptionFormAsync([FromBody] CreateAdoptFormRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await adoptionFormService.SubmitAdoptionFormAsync(request.ToAdoptionForm(), userId);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Đơn nhận nuôi đã được gửi thành công." }),
                Problem
            );
        }

        /// <summary>
        ///     Customer: Get all adoption forms submitted by the current user, order by submitted date descending.
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> GetAdoptionFormsAsync()
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var forms = await adoptionFormService.GetAdoptionFormsByUserIdAsync(userId);
            return Ok(forms);
        }

        /// <summary>
        ///     Customer, Organization: Get details of a specific adoption form by its ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [Authorize(Roles = $"{nameof(Roles.Customer)}, {nameof(Roles.Organization)}")]
        public async Task<IActionResult> GetAdoptionFormDetailsAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var form = await adoptionFormService.GetAdoptionFormDetailsByIdAsync(id, userId);
            return form.Match<IActionResult>(
                Ok,
                Problem
            );
        }

        /// <summary>
        ///     Customer: Cancel an adoption form by its ID. Only the user who submitted the form can cancel it.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}/cancel")]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> CancelAdoptionFormAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await adoptionFormService.CancelAdoptionFormAsync(id, userId);
            return result
                ? Ok(new { Message = "Đơn nhận nuôi đã được hủy thành công." })
                : Problem(
                    statusCode: 400,
                    title: "Không thể hủy đơn nhận nuôi",
                    detail: "Đơn nhận nuôi không thể hủy hoặc đã được xử lý."
                );
        }

        /// <summary>
        ///     Customer: Update an existing adoption form. Only the user who submitted the form can update it.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPut]
        [Authorize(Roles = nameof(Roles.Customer))]
        public async Task<IActionResult> UpdateAdoptionFormAsync([FromBody] UpdateAdoptFormRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await adoptionFormService.UpdateAdoptionFormAsync(request.ToAdoptionForm(), userId);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Đơn nhận nuôi đã được cập nhật thành công." }),
                Problem
            );
        }

        /// <summary>
        ///     Organization: Approve an adoption form by its ID. Only the organization can approve the form.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpPatch("{id}/approve")]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> ApproveAdoptionFormAsync(int id)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result =
                await adoptionFormService.ApproveAdoptionFormAsync(id, userId);
            return result.Match<IActionResult>(
                _ => Ok(new { Message = "Đơn nhận nuôi đã được chấp nhận." }),
                Problem
            );
        }

        /// <summary>
        ///     Organization: Reject an adoption form by its ID. Only the organization can reject the form.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpPatch("{id}/reject")]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> RejectAdoptionFormAsync(int id, RejectAdoptFormRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var result = await adoptionFormService.RejectAdoptionFormAsync(id, userId, request.RejectReason);
            return result
                ? Ok(new { Message = "Đơn nhận nuôi đã bị từ chối." })
                : Problem(
                    statusCode: 400,
                    title: "Không thể từ chối đơn nhận nuôi",
                    detail: "Đơn nhận nuôi không thể từ chối hoặc đã được xử lý."
                );
        }

        /// <summary>
        ///     Organization: Get all adoption forms submitted to the organization
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        [HttpGet]
        [Route("/api/organization/[controller]")]
        [Authorize(Roles = nameof(Roles.Organization))]
        public async Task<IActionResult> GetAdoptionFormsByOrganizationAsync(
            [FromQuery] OrganAdoptFormListRequest request)
        {
            var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var requestDto = request.ToFormListRequestDto();
            var forms = await adoptionFormService.GetAdoptFormsForOrganizationAsync(requestDto, userId);
            return Ok(forms);
        }
    }
}