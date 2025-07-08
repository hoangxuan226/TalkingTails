using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Pets;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class PetService(IUnitOfWork unitOfWork, IDateTimeProvider dateTimeProvider, IFileService fileService)
        : IPetService
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
                (p.Status.Equals(PetStatus.Available)) &&
                (p.Organization.Organization!.Status.Equals(OrganizationStatus.Active));
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

        public async Task<OneOf<bool, IError>> CreatePetAsync(CreatePetRequestDto requestDto)
        {
            try
            {
                var slug = await GetSlug(requestDto.PetName);

                var pet = new Pet
                {
                    PetName = requestDto.PetName,
                    Species = requestDto.Species,
                    Breed = requestDto.Breed,
                    Age = requestDto.Age,
                    Weight = requestDto.Weight,
                    Gender = requestDto.Gender,
                    Description = requestDto.Description,
                    LivingEnvironmentNeeds = requestDto.LivingEnvironmentNeeds,
                    Information = requestDto.Information,
                    Slug = slug,
                    CreatedAt = dateTimeProvider.UtcNow,
                    UpdatedAt = dateTimeProvider.UtcNow,
                    Status = PetStatus.Available,
                    OrganizationId = requestDto.OrganizationId,
                    PetImages = new List<PetImage>()
                };

                foreach (var file in requestDto.PetImages)
                {
                    var imageUrl = await fileService.UploadAsync(file);
                    pet.PetImages.Add(new PetImage { ImageUrl = imageUrl });
                }

                await unitOfWork.GenericRepository<Pet>().InsertAsync(pet);
                await unitOfWork.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi thêm vật nuôi.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<OneOf<bool, IError>> UpdatePetAsync(UpdatePetRequestDto requestDto)
        {
            try
            {
                var pet = await unitOfWork.GenericRepository<Pet>().GetAsync(
                    p => p.Id == requestDto.Id && p.OrganizationId == requestDto.OrganizationId, nameof(Pet.PetImages));
                if (pet == null) return new NotFoundError();

                // Identify images to delete
                var imagesToDelete = pet.PetImages
                    .Where(img => !requestDto.ExistingImageUrls.Contains(img.ImageUrl))
                    .ToList();

                if (imagesToDelete.Count == pet.PetImages.Count && requestDto.NewPetImages.Count == 0)
                {
                    return new InvalidResourcesError
                    {
                        Errors = new Dictionary<string, string[]>
                        {
                            {
                                nameof(Pet.PetImages),
                                ["Giữ lại ít nhất một hình ảnh hoặc thêm hình ảnh mới."]
                            }
                        }
                    };
                }

                var imageUrlsToDelete = imagesToDelete.Select(img => img.ImageUrl).ToList();

                // Remove from collection
                foreach (var imageToDelete in imagesToDelete)
                {
                    pet.PetImages.Remove(imageToDelete);
                }

                // Upload and add new images
                foreach (var newImageFile in requestDto.NewPetImages)
                {
                    var newImageUrl = await fileService.UploadAsync(newImageFile);
                    pet.PetImages.Add(new PetImage
                    {
                        ImageUrl = newImageUrl
                    });
                }

                // Update other pet properties
                pet.PetName = requestDto.PetName;
                pet.Species = requestDto.Species;
                pet.Breed = requestDto.Breed;
                pet.Age = requestDto.Age;
                pet.Weight = requestDto.Weight;
                pet.Gender = requestDto.Gender;
                pet.Description = requestDto.Description;
                pet.LivingEnvironmentNeeds = requestDto.LivingEnvironmentNeeds;
                pet.Information = requestDto.Information;
                pet.Slug = await GetSlug(requestDto.PetName);
                pet.UpdatedAt = dateTimeProvider.UtcNow;
                unitOfWork.GenericRepository<Pet>().Update(pet);
                await unitOfWork.SaveChangesAsync();

                // Only delete from storage after successful save
                foreach (var imageUrl in imageUrlsToDelete)
                {
                    await fileService.DeleteAsync(imageUrl);
                }

                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi cập nhật thú cưng.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<OneOf<bool, IError>> UpdatePetStatus(int id, string organizationId, PetStatus status)
        {
            if (status.Equals(PetStatus.Adopted))
            {
                return new UnsupportedOperationError
                    { Detail = "Chuyển trực tiếp sang trạng thái adopted chưa được hỗ trợ" };
            }

            var result = await unitOfWork.GenericRepository<Pet>()
                .ExecuteUpdateAsync(p => p.Id == id && p.OrganizationId.Equals(organizationId),
                    update => update.SetProperty(p => p.Status, status));
            return result > 0;
        }

        // Handle duplicate slug
        private async Task<string> GetSlug(string name)
        {
            var slug = name.ToSlug();
            var petIdBySlug = await unitOfWork.GenericRepository<Pet>().GetQueryable()
                .Where(p => p.Slug.Equals(slug)).Select(p => p.Id)
                .FirstOrDefaultAsync();
            return petIdBySlug != 0 ? $"{slug}-{petIdBySlug}" : slug;
        }
    }
}