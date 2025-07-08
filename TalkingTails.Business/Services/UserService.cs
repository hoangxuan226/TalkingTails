using System.Linq.Expressions;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Organizations;
using TalkingTails.Business.Models.Users;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Helpers;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class UserService(
        UserManager<ApplicationUser> userManager,
        IUnitOfWork unitOfWork,
        IDateTimeProvider dateTimeProvider,
        IFileService fileService) : IUserService
    {
        public async Task<dynamic?> GetAccountDetailsAsync(ClaimsPrincipal user)
        {
            var userId = user.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? "";
            var roles = user.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToList();
            if (roles.Contains(nameof(Roles.Organization)))
            {
                var organization = await unitOfWork.GenericRepository<ApplicationUser>()
                    .GetAsync<OrganizationDetailsDto>(u => u.Id.Equals(userId));
                return organization;
            }

            if (roles.Contains(nameof(Roles.Customer)))
            {
                var customer = await unitOfWork.GenericRepository<ApplicationUser>()
                    .GetAsync<CustomerDetailsDto>(u => u.Id.Equals(userId));
                return customer;
            }

            return null;
        }

        public async Task<OneOf<bool, IError>> UpdateAsync(EditCustomerRequestDto requestDto)
        {
            var user = await userManager.FindByIdAsync(requestDto.Id);
            if (user == null)
            {
                return new NotFoundError();
            }

            // Update email by SetEmailAsync to ensure email validation and uniqueness
            if (user.Email != requestDto.Email)
            {
                var updateEmailRs = await userManager.SetEmailAsync(user, requestDto.Email);
                if (!updateEmailRs.Succeeded)
                {
                    return new InvalidIdentityError(updateEmailRs.Errors);
                }
            }

            try
            {
                user.Address = requestDto.Address ?? user.Address;
                user.PhoneNumber = requestDto.PhoneNumber ?? user.PhoneNumber;
                user.Birthday = requestDto.Birthday ?? user.Birthday;
                user.UserName = requestDto.Email;
                user.Name = requestDto.Name;
                user.UpdatedAt = dateTimeProvider.UtcNow;
                var result = await userManager.UpdateAsync(user);
                if (!result.Succeeded)
                {
                    return new InvalidIdentityError(result.Errors);
                }

                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi cập nhật hồ sơ.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public Task<Pagination<AdminUserBasicDto>> GetAllForAdminAsync(AdminUserListRequestDto requestDto)
        {
            var pageIndex = requestDto.PageIndex ?? 1;
            var pageSize = requestDto.PageSize ?? 5;
            Expression<Func<ApplicationUser, bool>> filter = p =>
                (requestDto.SearchByName == null || p.Name == null || p.Name.Contains(requestDto.SearchByName)) &&
                (requestDto.SearchByEmail == null || p.Email == null || p.Email.Contains(requestDto.SearchByEmail));
            var repository = (IApplicationUserRepository)unitOfWork.GenericRepository<ApplicationUser>();
            repository.ApplyRole(Roles.Customer);
            return repository.GetPaginationAsync<AdminUserBasicDto>(pageIndex, pageSize, null, filter, requestDto.Sort);
        }

        public Task<CustomerDetailsDto?> GetCustomerDetailsAsync(string userId)
        {
            var repository = (IApplicationUserRepository)unitOfWork.GenericRepository<ApplicationUser>();
            repository.ApplyRole(Roles.Customer);
            return unitOfWork.GenericRepository<ApplicationUser>()
                .GetAsync<CustomerDetailsDto>(u => u.Id.Equals(userId));
        }

        public async Task<OneOf<bool, IError>> UpdateAvatarAsync(string userId, IFormFile avatar)
        {
            try
            {
                var user = await userManager.FindByIdAsync(userId);
                if (user == null)
                {
                    return new NotFoundError();
                }

                var newImageUrl = await fileService.UploadAsync(avatar);
                var oldImageUrl = user.ProfileImage;

                user.ProfileImage = newImageUrl;
                user.UpdatedAt = dateTimeProvider.UtcNow;
                var result = await userManager.UpdateAsync(user);

                if (!result.Succeeded)
                {
                    // Database update failed - clean up the uploaded image
                    await fileService.DeleteAsync(newImageUrl);
                    return new InvalidIdentityError(result.Errors);
                }

                if (!string.IsNullOrEmpty(oldImageUrl))
                {
                    await fileService.DeleteAsync(oldImageUrl);
                }

                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi thay đổi ảnh đại diện.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }
    }
}