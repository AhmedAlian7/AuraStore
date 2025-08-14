using E_Commerce.DataAccess.Entities;
using E_Commerce.DataAccess.Constants;
using Microsoft.AspNetCore.Identity;

namespace E_Commerce.DataAccess.Seeding
{
    public class IdentitySeeder
    {
        public static async Task SeedAsync(UserManager<ApplicationUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            foreach (var role in AppRoles.All)
                if (!await roleManager.RoleExistsAsync(role))
                    await roleManager.CreateAsync(new IdentityRole(role));

            var adminEmail = "admin@aurastore.com";
            var admin = await userManager.FindByEmailAsync(adminEmail);
            if (admin == null)
            {
                admin = new ApplicationUser
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true
                };
                var create = await userManager.CreateAsync(admin, "Admin#12345");
                if (create.Succeeded)
                    await userManager.AddToRoleAsync(admin, AppRoles.Admin);
            }
        }
    }

}
