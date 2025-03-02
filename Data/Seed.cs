using LearnerDuo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LearnerDuo.Data
{
    public class Seed
    {
        public static async Task SeedUsers(UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            if (await userManager.Users.AnyAsync()) return;

            try
            {
                var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
                var users = JsonSerializer.Deserialize<List<User>>(userData);
                if (users == null) return;

                var roles = new List<Role>
                {
                    new Role { Name = "Admin" },
                    new Role { Name = "Moderator" },
                    new Role { Name = "Member" }
                };

                foreach (var role in roles)
                {
                    await roleManager.CreateAsync(role); // add list role into database
                }

                foreach (var user in users)
                {
                    user.UserName = user.UserName.ToLower();

                    await userManager.CreateAsync(user, "Phucray@1310");

                    await userManager.AddToRoleAsync(user, "Member");

                }

                var admin = new User
                {
                    UserName = "admin",
                    Birthday = DateTime.Now,
                };

                await userManager.CreateAsync(admin, "Phucray@1310");
                // provide 2 value in ienumable in function AddToRolesAsync
                // it mean create random 2 user and each of that have role admin or morderator
                await userManager.AddToRolesAsync(admin, new[] { "Admin", "Moderator" });
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }

        }
    }
}
