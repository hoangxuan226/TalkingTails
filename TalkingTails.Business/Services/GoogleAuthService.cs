using System.Text.Json;
using Microsoft.Extensions.Options;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Authentication;
using TalkingTails.Business.Models.Setting;

namespace TalkingTails.Business.Services
{
    public class GoogleAuthService(HttpClient httpClient, IOptions<GoogleAuthSettings> googleOptions)
        : IGoogleAuthService
    {
        private readonly GoogleAuthSettings _googleSettings = googleOptions.Value;

        public async Task<OneOf<GoogleUserInfo, IError>> VerifyGoogleTokenAsync(string googleToken)
        {
            try
            {
                // Verify token with Google
                var tokenInfoUrl = $"{_googleSettings.TokenInfoUrl}?access_token={googleToken}";
                var response = await httpClient.GetAsync(tokenInfoUrl);

                if (!response.IsSuccessStatusCode)
                {
                    return new InvalidResourcesError { Detail = "Google token không hợp lệ" };
                }

                // Get user info from Google
                var userInfoUrl = $"{_googleSettings.UserInfoUrl}?access_token={googleToken}";
                var userInfoResponse = await httpClient.GetAsync(userInfoUrl);

                if (!userInfoResponse.IsSuccessStatusCode)
                {
                    return new InvalidResourcesError { Detail = "Không thể lấy thông tin người dùng từ Google" };
                }

                var userInfoJson = await userInfoResponse.Content.ReadAsStringAsync();
                var googleUser = JsonSerializer.Deserialize<JsonElement>(userInfoJson);

                var userInfo = new GoogleUserInfo
                {
                    Email = googleUser.GetProperty("email").GetString()!,
                    Name = googleUser.GetProperty("name").GetString()!,
                    Picture = googleUser.TryGetProperty("picture", out var picture) ? picture.GetString() : null,
                    GoogleId = googleUser.GetProperty("id").GetString()!
                };

                return userInfo;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = System.Net.HttpStatusCode.InternalServerError,
                    Detail = "Lỗi xác thực Google",
                    Errors = new Dictionary<string, string[]> { { "Exception", [ex.Message] } }
                };
            }
        }
    }
}