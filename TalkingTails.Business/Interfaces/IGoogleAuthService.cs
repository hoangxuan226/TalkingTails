using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Models.Authentication;

namespace TalkingTails.Business.Interfaces
{
    public interface IGoogleAuthService
    {
        Task<OneOf<GoogleUserInfo, IError>> VerifyGoogleTokenAsync(string googleToken);
    }
}