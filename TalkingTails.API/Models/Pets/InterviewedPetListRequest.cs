using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.Pets
{
    public class InterviewedPetListRequest
    {
        [Range(1, int.MaxValue)] public int? PageIndex { get; set; }
        [Range(1, 50)] public int? PageSize { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PetSpecies? FilterBySpecies { get; set; }

        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PetAgeCategories? FilterByAge { get; set; }

        public string? SearchByName { get; set; }
    }
}