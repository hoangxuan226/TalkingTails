using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Business.Services
{
    public class UserService(UserManager<ApplicationUser> userManager) : IUserService
    {
        public async Task<OneOf<ApplicationUser, IError>> CreateAsync(ApplicationUser user, string password,
            IList<string> roles)
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return new InvalidRegistrationError(result.Errors);
            }

            if (!roles.IsNullOrEmpty())
            {
                var roleResult = await userManager.AddToRolesAsync(user, roles);
                if (!roleResult.Succeeded)
                {
                    return new InvalidRegistrationError(roleResult.Errors);
                }
            }

            return user;
        }
    }
}