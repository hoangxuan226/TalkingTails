using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.AdoptionForms
{
    public class OrganAdoptFormListRequest
    {
        [Range(1, int.MaxValue)] public int? PageIndex { get; set; }
        [Range(1, 50)] public int? PageSize { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public FormStatus? FilterByStatus { get; set; }

        public string? Sort { get; set; }
    }
}