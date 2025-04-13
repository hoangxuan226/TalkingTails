using Microsoft.AspNetCore.Identity;
using TalkingTails.Repository.Constants;
using TalkingTails.Repository.Entities;

namespace TalkingTails.Repository.Data
{
    public class DataSeeder(
        RoleManager<IdentityRole> roleManager,
        UserManager<ApplicationUser> userManager
    )
    {
        public async Task SeedAsync()
        {
            await SeedRolesAsync();
            await SeedAdminUserAsync();
        }

        private async Task SeedRolesAsync()
        {
            string[] roles = [nameof(Roles.Admin), nameof(Roles.Customer)];
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
    }
}
