using GlobalBrandAssessment.DAL.Data.Models;
using GlobalBrandAssessment.GlobalBrandDbContext;
using Microsoft.AspNetCore.Identity;

namespace GlobalBrandAssessment.DAL.Seeding
{
    public static class ApplicationDbContextSeed
    {
        public static async Task SeedUsersAndRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager, GlobalbrandDbContext globalbrandDbContext)
        {
            // Roles
            if (!await roleManager.RoleExistsAsync("Manager"))
                await roleManager.CreateAsync(new IdentityRole("Manager"));

            if (!await roleManager.RoleExistsAsync("Employee"))
                await roleManager.CreateAsync(new IdentityRole("Employee"));

            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            // Users
            var users = new List<(string UserName, string Password, string Role, int? EmployeeId)>
            {
                ("AhmedFarag", "Ahmed2003#", "Manager", 1),
                ("AbdelrahmanMohammed","Abdelrahman123#","Employee",3),
                ("MariamAhmed", "Mariam123#", "Employee", 2),
                ("SaraAli", "SaraA123#", "Employee", 4),
                ("AliaaAli", "AliaaA123#", "Employee", 5),
                ("HamzaAli", "HamzaA123#", "Employee", 6),
                ("TarekSalama", "Tarek2003#", "Manager", 7),
                ("AliMohammed", "AliM2003#", "Manager", 8),
                ("MaiAlaa", "MaiA2003#", "Manager", 9),
                ("Admin", "Admin2003#", "Admin",null)
            };

            foreach (var (userName, password, role, employeeId) in users)
            {
                if (await userManager.FindByNameAsync(userName) == null)
                {


                    var user = new User
                    {
                        UserName = userName,
                        Email = $"{userName}@test.com",
                        EmailConfirmed = true,
                        EmployeeId = employeeId
                    };

                    var result = await userManager.CreateAsync(user, password);

                    if (result.Succeeded)
                    {
                        await userManager.AddToRoleAsync(user, role);

                    }

                }

            }
        }
    }
}
