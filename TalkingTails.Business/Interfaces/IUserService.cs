using System.Security.Claims;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.Users;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Business.Interfaces
{
    public interface IUserService
    {
        Task<OneOf<ApplicationUser, IError>> CreateAsync(ApplicationUser user, string password,
            IList<string> roles);

        Task<dynamic?> GetAccountDetailsAsync(ClaimsPrincipal user);
        Task<OneOf<bool, IError>> UpdateAsync(EditCustomerRequestDto requestDto);
    }
}