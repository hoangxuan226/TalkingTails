using System.Linq.Expressions;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Pets;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class PetService(IUnitOfWork unitOfWork) : IPetService
    {
        public Task<Pagination<OrganPetBasicDto>> GetPetsOfOrganizationAsync(PetListRequestDto requestDto,
            string organizationId)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            (string, string)? vietnameseSearch =
                requestDto.SearchByName == null ? null : (requestDto.SearchByName, nameof(Pet.PetName));
            Expression<Func<Pet, bool>> filter = p =>
                (requestDto.FilterByStatus == null || p.Status.Equals(requestDto.FilterByStatus)) &&
                (requestDto.FilterBySpecies == null || p.Species.Equals(requestDto.FilterBySpecies)) &&
                (requestDto.FilterByAge == null || p.Age.Equals(requestDto.FilterByAge));
            return unitOfWork.GenericRepository<Pet>().GetPaginationAsync<OrganPetBasicDto>(pageIndex, pageSize,
                vietnameseSearch, filter, requestDto.Sort);
        }

        public Task<Pagination<PetBasicDto>> GetPetsForGuestAsync(PetListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            (string, string)? vietnameseSearch =
                requestDto.SearchByName == null ? null : (requestDto.SearchByName, nameof(Pet.PetName));
            Expression<Func<Pet, bool>> filter = p =>
                (requestDto.FilterBySpecies == null || p.Species.Equals(requestDto.FilterBySpecies)) &&
                p.Status.Equals(PetStatus.Available);
            return unitOfWork.GenericRepository<Pet>().GetPaginationAsync<PetBasicDto>(pageIndex, pageSize,
                vietnameseSearch, filter);
        }

        public Task<GuestPetDetailsDto?> GetPetDetailsForGuestAsync(string slug)
        {
            return unitOfWork.GenericRepository<Pet>()
                .GetAsync<GuestPetDetailsDto>(p => p.Slug.Equals(slug) && p.Status.Equals(PetStatus.Available));
        }

        public Task<OrganPetDetailsDto?> GetPetDetailsForOrganizationAsync(int id, string organizationId)
        {
            return unitOfWork.GenericRepository<Pet>()
                .GetAsync<OrganPetDetailsDto>(p => p.Id == id && p.OrganizationId.Equals(organizationId));
        }

        public Task<Pagination<InterviewedPetDto>> GetPetWithRecentInterviewAsync(
            InterviewedPetListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            (string, string)? vietnameseSearch =
                requestDto.SearchByName == null ? null : (requestDto.SearchByName, nameof(Pet.PetName));
            Expression<Func<Pet, bool>> filter = p =>
                (requestDto.FilterBySpecies == null || p.Species.Equals(requestDto.FilterBySpecies)) &&
                (requestDto.FilterByAge == null || p.Age.Equals(requestDto.FilterByAge)) &&
                p.LastInterviewed != null && !p.Status.Equals(PetStatus.Adopted);
            var sort = $"{nameof(Pet.LastInterviewed)} desc";
            return unitOfWork.GenericRepository<Pet>()
                .GetPaginationAsync<InterviewedPetDto>(pageIndex, pageSize, vietnameseSearch, filter, sort);
        }
    }
}