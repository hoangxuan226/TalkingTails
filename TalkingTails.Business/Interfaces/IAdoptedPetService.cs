using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.AdoptedPets;

namespace TalkingTails.Business.Interfaces
{
    public interface IAdoptedPetService
    {
        Task<OneOf<bool, IError>> AdoptPetAsync(AdoptPetRequestDto requestDto);
        Task<List<CusAdoptedPetBasicDto>> GetAdoptedPetsByUserAsync(string userId);
    }
}