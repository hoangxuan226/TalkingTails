using Azure.Storage.Blobs;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalkingTails.Business.Interfaces;
using TalkingTails.Business.Models.Setting;
using TalkingTails.Business.Services;

namespace TalkingTails.Business
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddBusinessServices(this IServiceCollection services, IConfiguration config)
        {
            var azureConfiguration = config.GetSection(AzureStorageSettings.SectionName);
            var azureSettings = azureConfiguration.Get<AzureStorageSettings>();
            var connectionString = azureSettings?.ConnectionString;
            services.AddScoped(_ => new BlobServiceClient(connectionString));
            services.Configure<AzureStorageSettings>(azureConfiguration);
            services.Configure<JwtSettings>(config.GetSection(JwtSettings.SectionName));
            services.AddSingleton<IDateTimeProvider, DateTimeProvider>();
            services.AddScoped<IAuthService, AuthService>();
            services.AddScoped<IFileService, FileService>();
            return services;
        }
    }
}