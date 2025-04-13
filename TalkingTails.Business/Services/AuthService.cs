using System.IdentityModel.Tokens.Jwt;
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
        IUnitOfWork unitOfWork
    ) : IAuthService
    {
        public async Task<OneOf<AuthDto, IError>> RegisterAsync(
            ApplicationUser user,
            string password
        )
        {
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                var errorDictionary = new Dictionary<string, string[]>();
                foreach (var error in result.Errors)
                {
                    if (!error.Code.Equals("DuplicateUserName"))
                    {
                        errorDictionary[error.Code] = [error.Description];
                    }
                }
                return new InvalidRegistrationError { Errors = errorDictionary };
            }

            var role = nameof(Roles.Customer);
            await userManager.AddToRoleAsync(user, role);
            var refreshToken = await GenerateAndStoreRefreshToken(user);
            var (accessToken, accessTokenExpiration) = GenerateJwtToken(user);
            return new AuthDto(accessToken, accessTokenExpiration, refreshToken, user, [role]);
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
            var refreshToken = await GenerateAndStoreRefreshToken(user);
            var (accessToken, accessTokenExpiration) = GenerateJwtToken(user);
            return new AuthDto(accessToken, accessTokenExpiration, refreshToken, user, roles);
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
            var (accessToken, accessTokenExpiration) = GenerateJwtToken(user);
            return new AuthDto(accessToken, accessTokenExpiration, newRefreshToken, user, roles);
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

        private (string, DateTime) GenerateJwtToken(ApplicationUser user)
        {
            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            };

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Value.Key));
            var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expiration = dateTimeProvider.UtcNow.AddMinutes(jwtOptions.Value.AccessTokenExpireInMinutes);

            var token = new JwtSecurityToken(
                issuer: jwtOptions.Value.Issuer,
                audience: jwtOptions.Value.Audience,
                claims: claims,
                expires: expiration,
                signingCredentials: credentials
            );

            return (new JwtSecurityTokenHandler().WriteToken(token), expiration);
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
