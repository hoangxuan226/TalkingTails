using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.Organizations
{
    public class AdminOrganListRequest
    {
        [Range(1, int.MaxValue)] public int? PageIndex { get; set; }
        [Range(1, 50)] public int? PageSize { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public OrganizationStatus? FilterByStatus { get; set; }

        public string? SearchByName { get; set; }
        public string? SearchByEmail { get; set; }
        public string? Sort { get; set; }
    }
}