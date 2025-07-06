using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.AdoptedPets;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IAdoptedPetService
    {
        Task<OneOf<bool, IError>> AdoptPetAsync(AdoptPetRequestDto requestDto);
        Task<List<CusAdoptedPetBasicDto>> GetAdoptedPetsByUserAsync(string userId);
        Task<Pagination<AdminAdoptedPetBasicDto>> GetAdoptedPetsForAdminAsync(AdminAdoptedPetListRequestDto requestDto);
    }
}