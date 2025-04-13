using System.Net;

namespace TalkingTails.Business.Errors
{
    public class InvalidCredError : IError
    {
        public HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
        public string Detail => "Email hoặc mật khẩu không chính xác";
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}
