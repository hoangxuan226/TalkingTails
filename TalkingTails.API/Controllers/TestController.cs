using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Constants;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    //[Authorize]
    [ApiController]
    public class TestController(IFileService fileService) : ControllerBase
    {
        private const string CookieRefreshToken = "RefreshToken";

        [HttpGet("ping")]
        [Authorize(Roles = nameof(Roles.Admin))]
        //[Authorize]
        public IActionResult Ping()
        {
            var refreshToken = Request.Cookies[CookieRefreshToken];
            return Ok("Pong");
        }

        [HttpPost("upload-image")]
        public async Task<IActionResult> UploadImage(IFormFile image)
        {
            if (image.Length == 0)
            {
                return BadRequest("No file uploaded.");
            }

            var result = await fileService.UploadAsync(image);
            return Ok(result);
        }

        [HttpDelete("delete-image")]
        public async Task<IActionResult> DeleteImage(string imageUrl)
        {
            if (string.IsNullOrEmpty(imageUrl))
            {
                return BadRequest("No file URL provided.");
            }

            // Decode the URL in case it was encoded by the client
            var decodedUrl = System.Web.HttpUtility.UrlDecode(imageUrl);
            var result = await fileService.DeleteAsync(decodedUrl);
            return result ? Ok("File deleted successfully.") : NotFound("File not found.");
        }

        //// GET: api/<TestController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "value1", "value2" };
        //}

        //// GET api/<TestController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}

        //// POST api/<TestController>
        //[HttpPost]
        //public void Post([FromBody] string value) { }

        //// PUT api/<TestController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value) { }

        //// DELETE api/<TestController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id) { }
    }
}