using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using WebApplicationMVC.EntitiFramework.Entities;

namespace WebApplicationMVC.Services
{
    public class IdentityDataInitializer
    {

        public static async Task SeedAdminUserAsunc(UserManager<AppUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            const string adminRoleName = "Admin";
            const string adminUserName = "admin";
            const string adminEmail = "doncot44@yandex.ru";
            const string adminPassword = "User123!";

            if (!await roleManager.RoleExistsAsync(adminRoleName))
            {
                await roleManager.CreateAsync(new IdentityRole(adminRoleName));
            }

            var adminUser = await userManager.FindByNameAsync(adminUserName);

            if (adminUser == null) 
            {
                adminUser = new AppUser
                {
                    UserName = adminUserName,
                    Email = adminEmail,
                    EmailConfirmed = true
                };

                var createResult = await userManager.CreateAsync(adminUser, adminPassword);

                if (!createResult.Succeeded) 
                {
                    return;
                }

                if (!await userManager.IsInRoleAsync(adminUser, adminRoleName))
                {
                    await userManager.AddToRoleAsync(adminUser, adminRoleName);

                }
            }
        }
    }
}
