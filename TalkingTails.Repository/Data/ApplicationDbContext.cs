using System.Text.Json;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Repository.Data
{
    public class ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : IdentityDbContext<ApplicationUser>(options)
    {
        [DbFunction("normalize_vietnamese", Schema = "public")]
        public static string NormalizeVietnamese(string input) => throw new NotSupportedException();

        public DbSet<RefreshToken> RefreshTokens { get; set; } = null!;

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            #region Configure RefreshToken entity

            builder.Entity<RefreshToken>()
                .HasOne(rt => rt.User)
                .WithMany(u => u.RefreshTokens)
                .HasForeignKey(rt => rt.UserId);

            #endregion

            #region Configure OrganizationDetails as owned type of ApplicationUser

            builder.Entity<ApplicationUser>()
                .OwnsOne(u => u.Organization, _ => { }).Navigation(o => o.Organization).IsRequired(false);

            builder.Entity<Donation>()
                .HasOne(d => d.User)
                .WithMany(u => u.Donations)
                .HasForeignKey(d => d.UserId);

            builder.Entity<Donation>()
                .HasOne(d => d.Organization)
                .WithMany(o => o.DonationsReceived)
                .HasForeignKey(d => d.OrganizationId);

            #endregion

            #region Configure JSON fields

            builder.Entity<Pet>(entity =>
            {
                entity.Property(p => p.LivingEnvironmentNeeds).HasColumnType("jsonb").HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), // Serialize to JSON
                    v => JsonSerializer.Deserialize<string[]>(v, (JsonSerializerOptions?)null)! // Deserialize from JSON
                );
                entity.Property(p => p.Information).HasColumnType("jsonb").HasConversion(
                    v => JsonSerializer.Serialize(v, (JsonSerializerOptions?)null), // Serialize to JSON
                    v => JsonSerializer.Deserialize<List<PetInfoItem>>(v,
                        (JsonSerializerOptions?)null)! // Deserialize from JSON
                );
            });

            #endregion

            #region Configure unique index for Slug

            builder.Entity<Blog>()
                .HasIndex(b => b.Slug)
                .IsUnique();

            builder.Entity<Pet>()
                .HasIndex(p => p.Slug)
                .IsUnique();

            #endregion

            #region Configure enums as strings using value converters

            builder.Entity<Blog>()
                .Property(b => b.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (BlogStatus)Enum.Parse(typeof(BlogStatus), v))
                .HasColumnType("text");

            builder.Entity<Blog>()
                .Property(p => p.Species)
                .HasConversion(
                    v => v.ToString(),
                    v => (PetSpecies)Enum.Parse(typeof(PetSpecies), v))
                .HasColumnType("text");

            builder.Entity<Pet>()
                .Property(p => p.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (PetStatus)Enum.Parse(typeof(PetStatus), v))
                .HasColumnType("text");

            builder.Entity<Pet>()
                .Property(p => p.Species)
                .HasConversion(
                    v => v.ToString(),
                    v => (PetSpecies)Enum.Parse(typeof(PetSpecies), v))
                .HasColumnType("text");

            builder.Entity<Pet>()
                .Property(p => p.Gender)
                .HasConversion(
                    v => v.ToString(),
                    v => (Gender)Enum.Parse(typeof(Gender), v))
                .HasColumnType("text");

            builder.Entity<Pet>()
                .Property(p => p.Age)
                .HasConversion(
                    v => v.ToString(),
                    v => (PetAgeCategories)Enum.Parse(typeof(PetAgeCategories), v))
                .HasColumnType("text");

            builder.Entity<AdoptionForm>()
                .Property(a => a.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (FormStatus)Enum.Parse(typeof(FormStatus), v))
                .HasColumnType("text");

            builder.Entity<DonationPackage>()
                .Property(dp => dp.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (DonationPackageStatus)Enum.Parse(typeof(DonationPackageStatus), v))
                .HasColumnType("text");

            builder.Entity<InterviewSchedule>()
                .Property(i => i.Status)
                .HasConversion(
                    v => v.ToString(),
                    v => (InterviewScheduleStatus)Enum.Parse(typeof(InterviewScheduleStatus), v))
                .HasColumnType("text");

            #endregion

            #region Mapping database functions

            builder.HasDbFunction(typeof(ApplicationDbContext).GetMethod(nameof(NormalizeVietnamese)))
                .HasName("normalize_vietnamese");

            #endregion
        }
    };
}