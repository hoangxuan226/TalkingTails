using System.Net;

namespace TalkingTails.Business.Errors
{
    public class NotFoundError : IError
    {
        public HttpStatusCode StatusCode => HttpStatusCode.NotFound;
        public string Detail => "Không tìm thấy tài nguyên yêu cầu";
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}