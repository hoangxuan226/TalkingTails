using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.Organizations
{
    public class UpdateStatusRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrganizationStatus Status { get; set; }
    }
}