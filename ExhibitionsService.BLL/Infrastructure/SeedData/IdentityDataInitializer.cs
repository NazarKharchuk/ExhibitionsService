using ExhibitionsService.DAL.Entities;
using ExhibitionsService.DAL.Enums;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace ExhibitionsService.BLL.Infrastructure.SeedData
{
    public static class IdentityDataInitializer
    {
        public static async Task SeedData(IServiceProvider serviceProvider, IConfiguration configuration)
        {
            var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole<int>>>();

            foreach (Role role in Enum.GetValues(typeof(Role)))
            {
                if (!(await roleManager.RoleExistsAsync(role.ToString())))
                {
                    await roleManager.CreateAsync(new IdentityRole<int>(role.ToString()));
                }
            }

            var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

            var adminUser = new User
            {
                UserName = configuration["Admin:Email"],
                Email = configuration["Admin:Email"],
                UserProfile = new UserProfile
                {
                    FirstName = "AdminFirstName",
                    LastName = "AdminLastName",
                    JoiningDate = DateTime.UtcNow
                }
            };

            var _user = await userManager.FindByEmailAsync(adminUser.Email);

            if (_user == null)
            {
                var createPowerUser = await userManager.CreateAsync(adminUser, configuration["Admin:Password"]);
                if (createPowerUser.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, Role.Admin.ToString());
                }
            }
        }
    }
}
