using System.Linq.Expressions;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Organizations;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class OrganizationService(
        IUnitOfWork unitOfWork,
        IFileService fileService,
        UserManager<ApplicationUser> userManager,
        IDateTimeProvider dateTimeProvider)
        : IOrganizationService
    {
        public Task<List<OrganizationBasicDto>> GetAllForGuestAsync()
        {
            var repository = (IApplicationUserRepository)unitOfWork.GenericRepository<ApplicationUser>();
            repository.ApplyRole(Roles.Organization);
            return repository.GetAllAsync<OrganizationBasicDto>(o =>
                o.Organization!.Status.Equals(OrganizationStatus.Active));
        }

        public Task<Pagination<AdminOrganBasicDto>> GetAllForAdminAsync(AdminOrganListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            Expression<Func<ApplicationUser, bool>> filter = u =>
                (requestDto.FilterByStatus == null || u.Organization!.Status.Equals(requestDto.FilterByStatus)) &&
                (requestDto.SearchByName == null || u.Name == null || u.Name.Contains(requestDto.SearchByName)) &&
                (requestDto.SearchByEmail == null || u.Email == null || u.Email.Contains(requestDto.SearchByEmail));
            var repository = (IApplicationUserRepository)unitOfWork.GenericRepository<ApplicationUser>();
            repository.ApplyRole(Roles.Organization);
            return repository.GetPaginationAsync<AdminOrganBasicDto>(pageIndex, pageSize, null, filter,
                requestDto.Sort);
        }

        public Task<OrganizationDetailsDto?> GetDetailsAsync(string id)
        {
            var repository = (IApplicationUserRepository)unitOfWork.GenericRepository<ApplicationUser>();
            repository.ApplyRole(Roles.Organization);
            return repository.GetAsync<OrganizationDetailsDto>(u => u.Id.Equals(id));
        }

        public async Task<OneOf<bool, IError>> CreateAsync(CreateRequestDto requestDto)
        {
            try
            {
                var organization = new ApplicationUser()
                {
                    UserName = requestDto.Email,
                    Email = requestDto.Email,
                    Name = requestDto.Name,
                    PhoneNumber = requestDto.PhoneNumber,
                    Address = requestDto.Address,
                    Organization = new OrganizationDetails
                    {
                        Description = requestDto.Description,
                        Status = requestDto.Status,
                        TotalDonationAmount = 0
                    },
                    CreatedAt = dateTimeProvider.UtcNow,
                    UpdatedAt = dateTimeProvider.UtcNow,
                };

                if (requestDto.ProfileImage != null)
                {
                    var profileImageUrl = await fileService.UploadAsync(requestDto.ProfileImage);
                    organization.ProfileImage = profileImageUrl;
                }

                // Save the organization details
                var result = await userManager.CreateAsync(organization, requestDto.Password);
                if (!result.Succeeded)
                {
                    return new InvalidIdentityError(result.Errors);
                }

                // Assign the organization role
                var roleResult = await userManager.AddToRolesAsync(organization, [nameof(Roles.Organization)]);
                if (!roleResult.Succeeded)
                {
                    return new InvalidIdentityError(roleResult.Errors);
                }

                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi tạo tổ chức.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<OneOf<bool, IError>> UpdateAsync(UpdateRequestDto requestDto)
        {
            try
            {
                var organization = await userManager.FindByIdAsync(requestDto.Id);
                if (organization == null) return new NotFoundError();

                if (requestDto.ProfileImage != null)
                {
                    var profileImageUrl = await fileService.UploadAsync(requestDto.ProfileImage);
                    if (organization.ProfileImage != null) await fileService.DeleteAsync(organization.ProfileImage);
                    organization.ProfileImage = profileImageUrl;
                }

                // Update email by SetEmailAsync to ensure email validation and uniqueness
                if (organization.Email != requestDto.Email)
                {
                    var updateEmailRs = await userManager.SetEmailAsync(organization, requestDto.Email);
                    if (!updateEmailRs.Succeeded)
                    {
                        return new InvalidIdentityError(updateEmailRs.Errors);
                    }
                }

                organization.UserName = requestDto.Email;
                organization.Name = requestDto.Name;
                organization.PhoneNumber = requestDto.PhoneNumber;
                organization.Address = requestDto.Address;
                organization.Organization!.Description = requestDto.Description;
                organization.UpdatedAt = dateTimeProvider.UtcNow;
                organization.Organization.MeetLink = requestDto.MeetLink ?? organization.Organization.MeetLink;
                var updateResult = await userManager.UpdateAsync(organization);
                if (!updateResult.Succeeded)
                {
                    return new InvalidIdentityError(updateResult.Errors);
                }

                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi cập nhật tổ chức.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<OrganizationCountDto> GetOrganizationCountAsync()
        {
            var repository = (IApplicationUserRepository)unitOfWork.GenericRepository<ApplicationUser>();
            repository.ApplyRole(Roles.Organization);
            var result = await repository.GetQueryable()
                .GroupBy(o => o.Organization!.Status)
                .Select(g => new StatusCount
                {
                    StatusName = g.Key.ToString(),
                    Count = g.Count()
                }).ToListAsync();

            // Ensure all statuses are present, even with zero count
            var allStatuses = Enum.GetValues(typeof(OrganizationStatus)).Cast<OrganizationStatus>();
            foreach (var status in allStatuses)
            {
                if (result.All(x => x.StatusName != status.ToString()))
                {
                    result.Add(new StatusCount
                    {
                        StatusName = status.ToString(),
                        Count = 0
                    });
                }
            }

            return new OrganizationCountDto
            {
                StatusCount = result
            };
        }

        public async Task<bool> UpdateStatusAsync(string id, OrganizationStatus status)
        {
            var result = await unitOfWork.GenericRepository<ApplicationUser>().ExecuteUpdateAsync(u => u.Id == id,
                update => update.SetProperty(u => u.Organization!.Status, status));
            return result > 0;
        }
    }
}