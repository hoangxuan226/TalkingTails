using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TalkingTails.Repository.Data;
using TalkingTails.Repository.Interfaces;
using TalkingTails.Repository.Repositories;

namespace TalkingTails.Repository
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositoryServices(
            this IServiceCollection services,
            IConfiguration config
        )
        {
            // Configure PostgreSQL
            services.AddDbContext<ApplicationDbContext>(options =>
                options.UseNpgsql(config.GetConnectionString("DefaultConnection"))
            );

            services.AddScoped<DataSeeder>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            return services;
        }
    }
}
