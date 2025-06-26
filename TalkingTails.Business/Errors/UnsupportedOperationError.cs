using System.Net;

namespace TalkingTails.Business.Errors
{
    public class UnsupportedOperationError : IError
    {
        public HttpStatusCode StatusCode => HttpStatusCode.NotImplemented;
        public string Detail { get; set; } = "Chức năng này chưa được hỗ trợ.";
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}