using System.Reflection;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.OpenApi.Models;
using TalkingTails.API.Helpers;
using TalkingTails.Business;
using TalkingTails.Repository;
using TalkingTails.Repository.Data;

namespace TalkingTails.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers().AddJsonOptions(options =>
            {
                options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
            });

            // Add Services and Repositories
            builder.Services.AddRepositoryServices(builder.Configuration);
            builder.Services.AddBusinessServices(builder.Configuration);

            // Add CORS
            builder.Services.AddCors(options =>
            {
                var allowedOrigins = builder.Configuration["CORS_ALLOWED_ORIGINS"]?.Split(',')
                                     ?? ["http://localhost:5173"];

                options.AddPolicy(
                    "AllowReactApp",
                    corsPolicyBuilder =>
                    {
                        corsPolicyBuilder
                            .WithOrigins(allowedOrigins)
                            .AllowAnyHeader()
                            .AllowAnyMethod()
                            .AllowCredentials(); // Required for cookies (refresh token)
                    }
                );
            });

            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen(options =>
            {
                options.AddSecurityDefinition(
                    JwtBearerDefaults.AuthenticationScheme,
                    new OpenApiSecurityScheme
                    {
                        Name = "Authorization",
                        Description =
                            "Enter the Bearer Authorization string as following: `Bearer Generated-JWT-Token`",
                        In = ParameterLocation.Header,
                        Type = SecuritySchemeType.ApiKey,
                        Scheme = JwtBearerDefaults.AuthenticationScheme,
                    }
                );

                options.AddSecurityRequirement(
                    new OpenApiSecurityRequirement
                    {
                        {
                            new OpenApiSecurityScheme
                            {
                                Reference = new OpenApiReference
                                {
                                    Type = ReferenceType.SecurityScheme,
                                    Id = JwtBearerDefaults.AuthenticationScheme,
                                },
                                //Scheme = "Bearer",
                                Name = JwtBearerDefaults.AuthenticationScheme,
                                In = ParameterLocation.Header,
                            },
                            new List<string>()
                        },
                    }
                );

                // Use string representation for enums in Swagger
                options.UseOneOfForPolymorphism();
                options.UseAllOfForInheritance();
                options.SelectDiscriminatorNameUsing(type => "discriminator");
                options.SelectDiscriminatorValueUsing(type => type.Name);

                // This is the key part - it configures Swagger to use enum names (strings)
                options.SchemaFilter<SwaggerEnumSchemaFilter>();
                options.ParameterFilter<SwaggerEnumParameterFilter>();

                // using System.Reflection;
                var xmlFilename = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                options.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, xmlFilename));

                options.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo
                {
                    Title = "TalkingTails API",
                    Version = "v1",
                    Description =
                        @"<b>Sort usage (applies to all sort fields):</b><br/>
                        Pass column name (case-insensitive) to sort by asc, e.g. <code>CreatedAt</code>, <code>petName</code>.<br/>
                        Add <code>desc</code> to sort by desc, e.g. <code>CreatedAt desc</code>, <code>petName desc</code>.<br/>"
                });
            });

            builder.Services.AddEndpointsApiExplorer();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment() || builder.Configuration["EnableSwagger"] == "true")
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseCors("AllowReactApp");
            app.UseAuthentication();
            app.UseAuthorization();

            app.MapControllers();

            // Seed data
            using (var scope = app.Services.CreateScope())
            {
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                try
                {
                    await seeder.EnsureLastMigrationAsync();
                }
                catch (Exception ex)
                {
                    app.Logger.LogError(ex, "Failed to apply pending EF Core migrations on startup");
                }
                await seeder.SeedAsync();
            }

            await app.RunAsync();
        }
    }
}
