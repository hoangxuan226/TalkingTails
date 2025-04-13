using System.Net;

namespace TalkingTails.Business.Errors
{
    public class InvalidRegistrationError : IError
    {
        public HttpStatusCode StatusCode => HttpStatusCode.UnprocessableEntity;
        public string Detail { get; set; } = "Thông tin không hợp lệ";
        public IDictionary<string, string[]> Errors { get; set; } =
            new Dictionary<string, string[]>();
    }
}
