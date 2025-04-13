using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.Authentication;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Business.Interfaces
{
    public interface IAuthService
    {
        Task<OneOf<AuthDto, IError>> RegisterAsync(
            ApplicationUser user,
            string password
        );
        Task<OneOf<AuthDto, IError>> LoginAsync(string email, string password);
        Task<OneOf<AuthDto, IError>> RefreshTokenAsync(string refreshToken);
        Task<OneOf<bool, IError>> RevokeRefreshTokenAsync(string refreshToken);
    }
}
