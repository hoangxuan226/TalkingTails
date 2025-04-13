using System.Net;

namespace TalkingTails.Business.Errors
{
    public class MissingRefreshTokenError : IError
    {
        public HttpStatusCode StatusCode => HttpStatusCode.BadRequest;
        public string Detail => "Refresh token bị thiếu";
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
    }
}
