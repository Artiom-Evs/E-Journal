using Microsoft.AspNetCore.Identity;
using E_Journal.WebUI.Models;

namespace E_Journal.WebUI.Data
{
    public static class IdentitySeedData
    {
        private static async Task CheckRoleExistance(IdentityRole role, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync(role.Name))
            {
                var result = await roleManager.CreateAsync(role);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create {role.Name} role. Errors: \r\n{string.Join("\r\n", result.Errors.Select(e => $"- {e.Description}"))}");
                }
            }
        }

        public static async Task EnsurePopulated(IApplicationBuilder app)
        {
            IConfiguration configuration = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<IConfiguration>();
            RoleManager<IdentityRole> roleManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            UserManager<ApplicationUser> userManager = app.ApplicationServices.CreateScope().ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();

            IdentityRole adminRole = new(ApplicationRoles.Admin);
            IdentityRole teacherRole = new(ApplicationRoles.Teacher);
            IdentityRole studentRole = new(ApplicationRoles.Student);

            await CheckRoleExistance(adminRole, roleManager);
            await CheckRoleExistance(teacherRole, roleManager);
            await CheckRoleExistance(studentRole, roleManager);

            string name = configuration["WebUIDefaultAdmin:Name"];
            string password = configuration["WebUIDefaultAdmin:Password"];

            var defaultAdminUser = await userManager.FindByNameAsync(name);
            
            if (defaultAdminUser == null)
            {
                var result = await userManager.CreateAsync(new ApplicationUser(name), password);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to create default admin user. Errors: \r\n{string.Join("\r\n", result.Errors.Select(e => $"- {e.Description}\r\n"))}");
                }
            }

            defaultAdminUser = await userManager.FindByNameAsync(name);

            if (!await userManager.IsInRoleAsync(defaultAdminUser, adminRole.Name))
            {
                var result = await userManager.AddToRoleAsync(defaultAdminUser, adminRole.Name);

                if (!result.Succeeded)
                {
                    throw new InvalidOperationException($"Failed to add admin role to default admin user. Errors: \r\n{string.Join("\r\n", result.Errors.Select(e => $"- {e.Description}\r\n"))}");
                }
            }
        }
    }
}
