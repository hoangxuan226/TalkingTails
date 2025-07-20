using System.Text;
using Azure.Storage.Blobs;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Net.payOS;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Setting;
using TalkingTails.Business.Services;
using TalkingTails.Repository.Data;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration config)
        {
            #region Add azure Blob Storage

            var azureConfiguration = config.GetSection(AzureStorageSettings.SectionName);
            services.Configure<AzureStorageSettings>(azureConfiguration);
            var azureSettings = azureConfiguration.Get<AzureStorageSettings>() ??
                                throw new Exception("Cannot find Azure environment");
            var connectionString = azureSettings.ConnectionString;
            services.AddScoped(_ => new BlobServiceClient(connectionString));

            #endregion

            #region Add PayOs settings

            var payOsSettings = config.GetSection(PayOsSettings.SectionName).Get<PayOsSettings>() ??
                                throw new Exception("Cannot find PayOS environment");
            var payOs = new PayOS(payOsSettings.ClientId,
                payOsSettings.ApiKey, payOsSettings.ChecksumKey);
            services.AddSingleton(payOs);

            #endregion

            #region Configure Identity (MUST BE BEFORE AUTHENTICATION)

            services.AddIdentity<ApplicationUser, IdentityRole>(options =>
                {
                    options.Password.RequireDigit = true;
                    options.Password.RequiredLength = 6;
                    options.Password.RequireNonAlphanumeric = true;
                    options.Password.RequireUppercase = true;
                    options.Password.RequireLowercase = true;
                    options.User.RequireUniqueEmail = true;
                    options.SignIn.RequireConfirmedEmail = false; // Set to true later
                })
                .AddEntityFrameworkStores<ApplicationDbContext>()
                .AddDefaultTokenProviders();

            #endregion

            #region Add Authentication

            // Add JWT
            var jwtConfiguration = config.GetSection(JwtSettings.SectionName);
            var jwtSettings = jwtConfiguration.Get<JwtSettings>() ??
                              throw new Exception("Cannot find JWT environment");

            // Add Google Auth
            var googleAuthConfiguration = config.GetSection(GoogleAuthSettings.SectionName);
            var googleAuthSettings = googleAuthConfiguration.Get<GoogleAuthSettings>() ??
                                     throw new Exception("Cannot find Google Auth environment");

            services.Configure<JwtSettings>(jwtConfiguration);
            services.Configure<GoogleAuthSettings>(googleAuthConfiguration);
            services.AddScoped<IGoogleAuthService, GoogleAuthService>();
            services.AddHttpClient<IGoogleAuthService, GoogleAuthService>();
            services.AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = new TokenValidationParameters
                    {
                        ValidateIssuer = true,
                        ValidateAudience = true,
                        ValidateLifetime = true,
                        ValidateIssuerSigningKey = true,
                        ValidIssuer = jwtSettings.Issuer,
                        ValidAudience = jwtSettings.Audience,
                        IssuerSigningKey = new SymmetricSecurityKey(
                            Encoding.UTF8.GetBytes(jwtSettings.Key)
                        ),
                        ClockSkew = TimeSpan.Zero, // Remove delay 5 minutes of default
                    };
                })
                .AddGoogle(options =>
                {
                    options.ClientId = googleAuthSettings.ClientId;
                    options.ClientSecret = googleAuthSettings.ClientSecret;
                    options.SaveTokens = true;

                    // Request additional scopes if needed
                    options.Scope.Add("profile");
                    options.Scope.Add("email");
                });
            services.AddAuthorization();

            #endregion

            #region Add Email settings

            services.Configure<EmailSettings>(config.GetSection(EmailSettings.SectionName));
            services.AddScoped<IEmailService, EmailService>();

            #endregion

            #region Add other services

            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFileService, FileService>();
            services.AddScoped<IDonationService, DonationService>();
            services.AddScoped<IDonationPackageService, DonationPackageService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IPetService, PetService>();
            services.AddScoped<IBlogService, BlogService>();
            services.AddScoped<IAdoptionFormService, AdoptionFormService>();
            services.AddScoped<IInterviewScheduleService, InterviewScheduleService>();
            services.AddScoped<IAdoptedPetService, AdoptedPetService>();
            services.AddScoped<IOrganizationService, OrganizationService>();
            services.AddScoped<IBlogCommentService, BlogCommentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IDashboardService, DashboardService>();

            #endregion

            return services;
        }
    }
}