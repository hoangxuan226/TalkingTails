using System.Linq.Expressions;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.AdoptedPets;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class AdoptedPetService(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider) : IAdoptedPetService
    {
        public async Task<OneOf<bool, IError>> AdoptPetAsync(AdoptPetRequestDto requestDto)
        {
            var createDto = await unitOfWork.GenericRepository<InterviewSchedule>()
                .GetAsync<CreateAdoptedPetDto>(i =>
                    i.Id == requestDto.InterviewScheduleId &&
                    i.AdoptionForm.Pet.OrganizationId.Equals(requestDto.OrganizationId));
            if (createDto == null) return new NotFoundError();

            var pet = await unitOfWork.GenericRepository<Pet>()
                .GetAsync(p => p.Id == createDto.PetId);
            if (pet == null) return new NotFoundError();

            try
            {
                var adoptedPet = new AdoptedPet
                {
                    PetId = createDto.PetId,
                    AdopterId = createDto.AdopterId,
                    AdoptionFormId = createDto.AdoptionFormId,
                    CreatedAt = dateTimeProvider.UtcNow
                };
                await unitOfWork.GenericRepository<AdoptedPet>().InsertAsync(adoptedPet);
                pet.Status = PetStatus.Adopted;
                unitOfWork.GenericRepository<Pet>().Update(pet);
                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi chọn chủ cho vật nuôi.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public Task<List<CusAdoptedPetBasicDto>> GetAdoptedPetsByUserAsync(string userId)
        {
            return unitOfWork.GenericRepository<AdoptedPet>()
                .GetAllAsync<CusAdoptedPetBasicDto>(a => a.AdopterId.Equals(userId));
        }

        public Task<Pagination<AdminAdoptedPetBasicDto>> GetAdoptedPetsForAdminAsync(
            AdminAdoptedPetListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            Expression<Func<AdoptedPet, bool>> filter = ap =>
                (requestDto.FilterByStartDate == null || ap.CreatedAt >= requestDto.FilterByStartDate)
                && (requestDto.FilterByEndDate == null || ap.CreatedAt <= requestDto.FilterByEndDate);
            return unitOfWork.GenericRepository<AdoptedPet>()
                .GetPaginationAsync<AdminAdoptedPetBasicDto>(pageIndex: pageIndex, pageSize: pageSize,
                    predicate: filter,
                    sort: requestDto.Sort);
        }
    }
}