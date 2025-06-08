using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;
using TalkingTails.Repository.Interfaces;

namespace TalkingTails.Repository.Data
{
    public class DataSeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager,
        ILogger<DataSeeder> logger,
        IUnitOfWork unitOfWork
    )
    {
        public async Task SeedAsync()
        {
            #region Seeding Roles

            try
            {
                await SeedRolesAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding roles.");
            }

            #endregion

            #region Seeding Admin User

            try
            {
                await SeedAdminUserAsync();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding admin user.");
            }

            #endregion

            #region Seeding Donation Packages

            try
            {
                await SeedDonationPackages();
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Error occurred while seeding donation packages.");
            }

            #endregion
        }

        private async Task SeedRolesAsync()
        {
            string[] roles = [nameof(Roles.Admin), nameof(Roles.Customer), nameof(Roles.Organization)];
            foreach (var role in roles)
            {
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }
        }

        private async Task SeedAdminUserAsync()
        {
            var adminUser = new ApplicationUser
            {
                UserName = "admin@example.com",
                Email = "admin@example.com",
            };

            var adminExists = await userManager.FindByEmailAsync(adminUser.Email);
            if (adminExists == null)
            {
                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, nameof(Roles.Admin));
                }
            }
            else if (!await userManager.IsInRoleAsync(adminExists, nameof(Roles.Admin)))
            {
                await userManager.AddToRoleAsync(adminExists, nameof(Roles.Admin));
            }
        }

        private async Task SeedDonationPackages()
        {
            var donationPackages = new List<DonationPackage>
            {
                new DonationPackage
                {
                    Id = 1,
                    Name = "Người đồng hành",
                    Amount = 50000,
                    Description = "Giúp chúng mình cung cấp thức ăn cho một thú cưng trong 1 tuần",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = DonationPackageStatus.Active
                },
                new DonationPackage
                {
                    Id = 2,
                    Name = "Người bảo vệ",
                    Amount = 200000,
                    Description = "Hỗ trợ chi phí y tế và chăm sóc cơ bản cho thú cưng",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = DonationPackageStatus.Active
                },
                new DonationPackage
                {
                    Id = 3,
                    Name = "Người hùng",
                    Amount = 1000000,
                    Description = "Đóng góp to lớn cho hoạt động cứu hộ và chăm sóc",
                    CreatedAt = DateTime.UtcNow,
                    UpdatedAt = DateTime.UtcNow,
                    Status = DonationPackageStatus.Active
                }
            };

            var isSeeding = false;
            var donationPackageRepository = unitOfWork.GenericRepository<DonationPackage>();
            foreach (var package in donationPackages)
            {
                if (!await donationPackageRepository.AnyAsync(dp => dp.Id == package.Id))
                {
                    package.Id = 0; // Reset id to allow EF Core to generate a new one
                    await donationPackageRepository.InsertAsync(package);
                    isSeeding = true;
                }
            }

            if (isSeeding)
            {
                await unitOfWork.SaveChangesAsync();
            }
        }
    }
}