using System.Net;

namespace TalkingTails.Business.Errors
{
    public class ForbiddenError : IError
    {
        public HttpStatusCode StatusCode => HttpStatusCode.Forbidden;
        public string Detail => "Bạn không có quyền truy cập vào tài nguyên này.";
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}