using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Net.payOS;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Setting;
using TalkingTails.Business.Services;

namespace TalkingTails.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration config)
        {
            // Add azure Blob Storage
            var azureConfiguration = config.GetSection(AzureStorageSettings.SectionName);
            services.Configure<AzureStorageSettings>(azureConfiguration);
            var azureSettings = azureConfiguration.Get<AzureStorageSettings>() ??
                                throw new Exception("Cannot find Azure environment");
            var connectionString = azureSettings.ConnectionString;
            services.AddScoped(_ => new BlobServiceClient(connectionString));

            // Add PayOs settings
            //var payOsConfiguration = config.GetSection(PayOsSettings.SectionName);
            //services.Configure<PayOsSettings>(config.GetSection(PayOsSettings.SectionName));
            var payOsSettings = config.GetSection(PayOsSettings.SectionName).Get<PayOsSettings>() ??
                                throw new Exception("Cannot find PayOS environment");
            var payOs = new PayOS(payOsSettings.ClientId,
                payOsSettings.ApiKey, payOsSettings.ChecksumKey);
            services.AddSingleton(payOs);

            // Add JWT settings
            services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));

            // Add Email settings
            services.Configure<EmailSettings>(config.GetSection(EmailSettings.SectionName));
            services.AddScoped<IEmailService, EmailService>();

            // Add other services
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
            return services;
        }
    }
}