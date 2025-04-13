using System.Net;

namespace TalkingTails.Business.Errors
{
    public interface IError
    {
        public HttpStatusCode StatusCode { get; }
        public string Detail { get; }
        public IDictionary<string, string[]> Errors { get; }
    }
}
