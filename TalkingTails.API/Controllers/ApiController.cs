using Microsoft.AspNetCore.Mvc;
using TalkingTails.Business.Errors;

namespace TalkingTails.API.Controllers
{
    public class ApiController : ControllerBase
    {
        protected IActionResult Problem(IError error)
        {
            foreach (var kvPair in error.Errors)
            {
                foreach (var errorMessage in kvPair.Value)
                {
                    ModelState.AddModelError(kvPair.Key, errorMessage);
                }
            }

            if (!ModelState.IsValid)
            {
                return ValidationProblem(
                    statusCode: (int)error.StatusCode,
                    detail: error.Detail,
                    modelStateDictionary: ModelState
                );
            }

            return Problem(statusCode: (int)error.StatusCode, detail: error.Detail);
        }
    }
}