using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.Users;
using TalkingTails.Repository.Helpers;

namespace TalkingTails.Business.Interfaces
{
    public interface IUserService
    {
        Task<dynamic?> GetAccountDetailsAsync(ClaimsPrincipal user);
        Task<OneOf<bool, IError>> UpdateAsync(EditCustomerRequestDto requestDto);
        Task<Pagination<AdminUserBasicDto>> GetAllForAdminAsync(AdminUserListRequestDto requestDto);
        Task<CustomerDetailsDto?> GetCustomerDetailsAsync(string userId);
        Task<OneOf<bool, IError>> UpdateAvatarAsync(string userId, IFormFile avatar);
    }
}