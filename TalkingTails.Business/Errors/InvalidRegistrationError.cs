using System.Net;
using Microsoft.AspNetCore.Identity;

namespace TalkingTails.Business.Errors
{
    public class InvalidRegistrationError : IError
    {
        public HttpStatusCode StatusCode => HttpStatusCode.UnprocessableEntity;
        public string Detail { get; set; } = "Thông tin không hợp lệ";
        public IDictionary<string, string[]> Errors { get; set; } =
            new Dictionary<string, string[]>();

        public InvalidRegistrationError(IEnumerable<IdentityError> errors)
        {
            foreach (var error in errors)
            {
                if (error.Code.Equals("DuplicateUserName")) continue;
                if (error.Code.Equals("DuplicateEmail"))
                {
                    error.Description = "Email này đã có người sử dụng";
                }
                
                Errors[error.Code] = [error.Description];
            }
        }
    }
}
