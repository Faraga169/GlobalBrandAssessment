//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using GlobalBrandAssessment.DAL.Data.Models;
//using Microsoft.AspNetCore.Identity;

//namespace GlobalBrandAssessment.DAL.Data
//{
//   public static class ApplicationDbContextSeed
//    {
//        public static async Task SeedUsersAndRolesAsync(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
//        {
//            // Roles
//            if (!await roleManager.RoleExistsAsync("Manager"))
//                await roleManager.CreateAsync(new IdentityRole("Manager"));

//            if (!await roleManager.RoleExistsAsync("Employee"))
//                await roleManager.CreateAsync(new IdentityRole("Employee"));

//            // Users
//            var users = new List<(string UserName, string Password, string Role, int EmployeeId)>
//        {
//            ("AhmedFarag", "Ahmed2003#", "Manager", 1),
//            ("MariamAhmed", "Mariam123#", "Employee", 2),
//            ("AbdelrahmanMohammed", "Abdelrahman123#", "Employee", 3),
//            ("SaraAli", "Sara123#", "Employee", 4),
//            ("AliaaAli", "Aliaa123#", "Employee", 5),
//            ("HamzaAli", "Hamza123#", "Employee", 6),
//            ("TarekSalama", "Tarek2003#", "Manager", 7),
//            ("AliMohammed", "Ali2003#", "Manager", 8),
//            ("MaiAlaa", "Mai2003#", "Manager", 9)
//        };

//            foreach (var (userName, password, role, employeeId) in users)
//            {
//                if (await userManager.FindByNameAsync(userName) == null)
//                {
//                    var user = new User { UserName = userName, Email = $"{userName}@test.com", EmailConfirmed = true,EmployeeId=employeeId };
//                    var result = await userManager.CreateAsync(user, password);
//                    if (result.Succeeded)
//                    {
//                        await userManager.AddToRoleAsync(user, role); 
//                    }
//                }
//            }
//        }
//    }
//}
