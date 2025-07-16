using Microsoft.AspNetCore.Identity;
using SalesPoint.Enum;
using SalesPoint.Models;

namespace SalesPoint.Data
{
    public static class IdentitySeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {
            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var config = serviceProvider.GetRequiredService<IConfiguration>();

            foreach (var roleName in System.Enum.GetNames(typeof(UserRole)))
            {
                if (!await roleManager.RoleExistsAsync(roleName))
                {
                    await roleManager.CreateAsync(new IdentityRole(roleName));
                }
            }

            string adminEmail = config["SeedAdmin:Email"] ?? "admin@localhost.com";
            string adminPassword = config["SeedAdmin:Password"] ?? "Admin123!";

            var adminUser = await userManager.FindByEmailAsync(adminEmail);
            if (adminUser == null)
            {
                var user = new User
                {
                    UserName = adminEmail,
                    Email = adminEmail,
                    EmailConfirmed = true,
                    FirstName = "Admin",
                    LastName = "User",
                    EmployeeId = "EMP-ADMIN-0001",
                    Role = UserRole.Admin,
                };

                var result = await userManager.CreateAsync(user, adminPassword);
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(user, UserRole.Admin.ToString());
                }
                else
                {
                    throw new Exception("Admin user creation failed: " + string.Join(", ", result.Errors.Select(e => e.Description)));
                }
            }
        }
    }
}
