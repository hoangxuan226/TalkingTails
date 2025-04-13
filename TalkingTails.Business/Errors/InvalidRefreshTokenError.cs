using System.Net;

namespace TalkingTails.Business.Errors
{
    public class InvalidRefreshTokenError : IError
    {
        public HttpStatusCode StatusCode => HttpStatusCode.Unauthorized;
        public string Detail => "Refresh token không hợp lệ";
        public IDictionary<string, string[]> Errors { get; } = new Dictionary<string, string[]>();
    }
}
