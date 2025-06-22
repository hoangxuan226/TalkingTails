using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.Blogs
{
    public class UpdateBlogStatusRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public BlogStatus Status { get; set; }
    }
}