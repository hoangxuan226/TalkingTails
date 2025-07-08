using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using OneOf;
using TalkingTails.Business.Errors;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Authentication;
using TalkingTails.Business.Models.Setting;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Business.Services
{
    public class AuthService(
        IOptions<JwtSettings> jwtOptions,
        IDateTimeProvider dateTimeProvider,
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        IUnitOfWork unitOfWork,
        IEmailService emailService
    ) : IAuthService
    {
        public async Task<OneOf<AuthDto, IError>> RegisterAsync(
            ApplicationUser user,
            string password
        )
        {
            user.CreatedAt = dateTimeProvider.UtcNow;
            user.UpdatedAt = dateTimeProvider.UtcNow;
            user.Customer = new CustomerDetails
            {
                Status = CustomerStatus.Active,
                TotalDonatedAmount = 0
            };

            // Save the user details
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                return new InvalidIdentityError(result.Errors);
            }

            // Assign the default role to the user
            var role = nameof(Roles.Customer);
            await userManager.AddToRoleAsync(user, role);
            var refreshToken = await GenerateAndStoreRefreshToken(user);
            return new AuthDto
            {
                AccessToken = GenerateJwtToken(user, [role]),
                RefreshToken = refreshToken,
                User = user,
                Roles = [role]
            };
        }

        public async Task<OneOf<AuthDto, IError>> LoginAsync(
            string email,
            string password
        )
        {
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
            {
                return new InvalidCredError();
            }

            var result = await signInManager.CheckPasswordSignInAsync(
                user,
                password,
                lockoutOnFailure: false
            );
            if (!result.Succeeded)
            {
                return new InvalidCredError();
            }

            var roles = await userManager.GetRolesAsync(user);
            if (roles.Contains(nameof(Roles.Customer)) && !user.Customer!.Status.Equals(CustomerStatus.Active))
            {
                return new ForbiddenError { Detail = "Tài khoản này đã bị vô hiệu hoá" };
            }

            if (roles.Contains(nameof(Roles.Organization)) &&
                !user.Organization!.Status.Equals(OrganizationStatus.Active))
            {
                return new ForbiddenError { Detail = "Tài khoản này đã bị vô hiệu hoá" };
            }

            var refreshToken = await GenerateAndStoreRefreshToken(user);
            return new AuthDto
            {
                AccessToken = GenerateJwtToken(user, roles),
                RefreshToken = refreshToken,
                User = user,
                Roles = roles
            };
        }

        public async Task<OneOf<AuthDto, IError>> RefreshTokenAsync(string refreshToken)
        {
            var storedToken = await unitOfWork.GenericRepository<RefreshToken>()
                .GetAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (storedToken == null || storedToken.ExpiresAt < dateTimeProvider.UtcNow)
            {
                return new InvalidRefreshTokenError();
            }

            var user = await userManager.FindByIdAsync(storedToken.UserId);
            if (user == null)
            {
                return new InvalidRefreshTokenError();
            }

            storedToken.IsRevoked = true;
            unitOfWork.GenericRepository<RefreshToken>().Update(storedToken);
            await unitOfWork.SaveChangesAsync();
            var newRefreshToken = await GenerateAndStoreRefreshToken(user);

            var roles = await userManager.GetRolesAsync(user);
            return new AuthDto
            {
                AccessToken = GenerateJwtToken(user, roles),
                RefreshToken = newRefreshToken,
                User = user,
                Roles = roles
            };
        }

        public async Task<OneOf<bool, IError>> RevokeRefreshTokenAsync(string refreshToken)
        {
            var storedToken = await unitOfWork.GenericRepository<RefreshToken>()
                .GetAsync(rt => rt.Token == refreshToken && !rt.IsRevoked);

            if (storedToken == null || storedToken.ExpiresAt < dateTimeProvider.UtcNow)
            {
                return new InvalidRefreshTokenError();
            }

            storedToken.IsRevoked = true;
            unitOfWork.GenericRepository<RefreshToken>().Update(storedToken);
            await unitOfWork.SaveChangesAsync();
            return true;
        }

        public async Task<OneOf<bool, IError>> ForgotPasswordAsync(string email)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(email);
                if (user == null)
                {
                    // Return success even if user doesn't exist (security best practice)
                    return true;
                }

                // Generate password reset token
                // Default expiration is 1 hour because of AddDefaultTokenProviders
                var resetToken = await userManager.GeneratePasswordResetTokenAsync(user);

                // Send email
                await emailService.SendPasswordResetEmailAsync(
                    user.Email!,
                    resetToken,
                    user.Name ?? "User");

                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi gửi email đặt lại mật khẩu.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }

        public async Task<OneOf<bool, IError>> ResetPasswordAsync(ResetPasswordRequestDto requestDto)
        {
            try
            {
                var user = await userManager.FindByEmailAsync(requestDto.Email);
                if (user == null)
                {
                    return new InvalidResourcesError
                    {
                        Detail = "Không tìm thấy người dùng với email này."
                    };
                }

                var result = await userManager.ResetPasswordAsync(user, requestDto.Token, requestDto.NewPassword);
                if (!result.Succeeded)
                {
                    return new InvalidIdentityError(result.Errors);
                }

                // Update user's UpdatedAt timestamp
                user.UpdatedAt = dateTimeProvider.UtcNow;
                await userManager.UpdateAsync(user);

                return true;
            }
            catch (Exception ex)
            {
                return new Error
                {
                    StatusCode = HttpStatusCode.InternalServerError,
                    Detail = "Đã xảy ra lỗi khi đặt lại mật khẩu.",
                    Errors = new Dictionary<string, string[]>
                    {
                        { "Exception", [ex.Message] }
                    }
                };
            }
        }


        private string GenerateJwtToken(ApplicationUser user, IList<string>? roles)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            if (roles != null)
            {
                claims.AddRange(roles.Select(role => new Claim(ClaimTypes.Role, role)));
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Value.Issuer,
                audience: jwtOptions.Value.Audience,
                claims: claims,
                expires: dateTimeProvider.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenExpireInMinutes),
                signingCredentials: credentials
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task<string> GenerateAndStoreRefreshToken(ApplicationUser user)
        {
            var refreshToken = new RefreshToken
            {
                Token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(32)), // Secure random token
                CreatedAt = dateTimeProvider.UtcNow,
                ExpiresAt = dateTimeProvider.UtcNow.AddMinutes(jwtOptions.Value.RefreshTokenExpireInMinutes),
                IsRevoked = false,
                UserId = user.Id
            };

            await unitOfWork.GenericRepository<RefreshToken>().InsertAsync(refreshToken);
            await unitOfWork.SaveChangesAsync();
            return refreshToken.Token;
        }
    }
}