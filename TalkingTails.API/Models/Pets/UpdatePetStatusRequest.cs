using System.Text.Json.Serialization;
using TalkingTails.Repository.Constants;

namespace TalkingTails.API.Models.Pets
{
    public class UpdatePetStatusRequest
    {
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public PetStatus Status { get; set; }
    }
}