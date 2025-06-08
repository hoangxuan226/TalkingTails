using System.Net;

namespace TalkingTails.Business.Errors
{
    public class Error : IError
    {
        public required HttpStatusCode StatusCode { get; set; }
        public required string Detail { get; set; }
        public IDictionary<string, string[]> Errors { get; set; } = new Dictionary<string, string[]>();
    }
}