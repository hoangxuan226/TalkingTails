using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Business.Interfaces
{
    public interface IUserService
    {
        Task<OneOf<ApplicationUser, IError>> CreateAsync(ApplicationUser user, string password,
            IList<string> roles);
    }
}