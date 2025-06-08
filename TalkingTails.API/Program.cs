using System.Reflection;
using System.Text;
using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using TalkingTails.API.Helpers;
using TalkingTails.Business;
using TalkingTails.Business.Models.Setting;
using TalkingTails.Repository;
using TalkingTails.Repository.Data;
using TalkingTails.Repository.Entities;

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

            // Configure Identity
            builder
                .Services.AddIdentity<ApplicationUser, IdentityRole>(options =>
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

            // Configure JWT Authentication
            var jwtSettings = new JwtSettings();
            builder.Configuration.Bind(JwtSettings.SectionName, jwtSettings);
            builder
                .Services.AddAuthentication(options =>
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
                });
            builder.Services.AddAuthorization();

            // Add CORS
            builder.Services.AddCors(options =>
            {
                options.AddPolicy(
                    "AllowReactApp",
                    corsPolicyBuilder =>
                    {
                        corsPolicyBuilder
                            .WithOrigins("http://localhost:5173") // Adjust for your React app's URL
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

            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
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
                //var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                //await dbContext.Database.EnsureCreatedAsync();
                var seeder = scope.ServiceProvider.GetRequiredService<DataSeeder>();
                await seeder.SeedAsync();
            }

            await app.RunAsync();
        }
    }
}