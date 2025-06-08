using System.Net;

namespace TalkingTails.Business.Errors
{
    public class PayOsError(Exception exception) : IError
    {
        public HttpStatusCode StatusCode => HttpStatusCode.InternalServerError;
        public string Detail { get; set; } = "Lỗi với PayOs";

        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>
        {
            { "Exception", [exception.Message] }
        };
    }
}