using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using TalkingTails.API.Helpers.Mappings;
using TalkingTails.API.Models.Authentication;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Setting;

namespace TalkingTails.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController(IAuthService authService, IOptions<JwtSettings> jwtOptions) : ApiController
    {
        private const string CookieRefreshToken = "RefreshToken";

        [HttpPost("register")]
        public async Task<IActionResult> Register([FromBody] RegisterRequest request)
        {
            var user = request.ToApplicationUser();
            var result = await authService.RegisterAsync(user, request.Password);
            return result.Match<IActionResult>(
                authDto =>
                {
                    SetRefreshTokenCookie(authDto.RefreshToken); // Set refresh token in cookie
                    return Ok(authDto.ToAuthResponse());
                },
                Problem
            );
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginRequest request)
        {
            var result = await authService.LoginAsync(request.Email, request.Password);
            return result.Match<IActionResult>(
                authDto =>
                {
                    SetRefreshTokenCookie(authDto.RefreshToken); // Set refresh token in cookie
                    return Ok(authDto.ToAuthResponse());
                },
                Problem
            );
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var refreshToken = Request.Cookies[CookieRefreshToken];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Problem(new MissingRefreshTokenError());
            }

            var result = await authService.RefreshTokenAsync(refreshToken);
            return result.Match<IActionResult>(
                authDto =>
                {
                    SetRefreshTokenCookie(authDto.RefreshToken); // Set refresh token in cookie
                    return Ok(authDto.ToAuthResponse());
                },
                Problem
            );
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies[CookieRefreshToken];
            if (string.IsNullOrEmpty(refreshToken))
            {
                return Problem(new MissingRefreshTokenError());
            }

            var result = await authService.RevokeRefreshTokenAsync(refreshToken);
            return result.Match<IActionResult>(
                success =>
                {
                    Response.Cookies.Delete(CookieRefreshToken); // Delete the refresh token cookie
                    return Ok(new { message = "Đăng xuất thành công" });
                },
                Problem
            );
        }

        [HttpPost("forgot-password")]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var result = await authService.ForgotPasswordAsync(request.Email);
            return result.Match<IActionResult>(
                _ => Ok(new { message = "Chúng tôi đã gửi liên kết đặt lại mật khẩu." }),
                Problem
            );
        }

        [HttpPost("reset-password")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordRequest request)
        {
            var requestDto = request.ToResetPasswordRequestDto();
            var result = await authService.ResetPasswordAsync(requestDto);
            return result.Match<IActionResult>(
                _ => Ok(new { message = "Đặt lại mật khẩu thành công." }),
                Problem
            );
        }

        private void SetRefreshTokenCookie(string refreshToken)
        {
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true, // Prevent JavaScript access
                Secure = true, // Only send over HTTPS
                //SameSite = SameSiteMode.Strict, // Prevent CSRF
                SameSite = SameSiteMode.None, // Allow cross-site cookies
                Expires = DateTime.UtcNow.AddMinutes(jwtOptions.Value
                    .RefreshTokenExpireInMinutes) // Match refresh token expiration
            };
            Response.Cookies.Append(CookieRefreshToken, refreshToken, cookieOptions);
        }
    }
}