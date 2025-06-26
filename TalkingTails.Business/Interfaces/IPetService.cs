using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.Pets;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IPetService
    {
        Task<Pagination<OrganPetBasicDto>> GetPetsOfOrganizationAsync(PetListRequestDto requestDto,
            string organizationId);

        Task<Pagination<PetBasicDto>> GetPetsForGuestAsync(PetListRequestDto requestDto);
        Task<GuestPetDetailsDto?> GetPetDetailsForGuestAsync(string slug);
        Task<OrganPetDetailsDto?> GetPetDetailsForOrganizationAsync(int id, string organizationId);
        Task<Pagination<InterviewedPetDto>> GetPetWithRecentInterviewAsync(InterviewedPetListRequestDto requestDto);
        Task<OneOf<bool, IError>> CreatePetAsync(CreatePetRequestDto requestDto);
        Task<OneOf<bool, IError>> UpdatePetAsync(UpdatePetRequestDto requestDto);
        Task<OneOf<bool, IError>> UpdatePetStatus(int id, string organizationId, PetStatus status);
    }
}