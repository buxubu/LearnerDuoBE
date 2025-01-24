using LearnerDuo.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LearnerDuo.Data
{
    public class Seed
    {
        public static async Task SeedUsers(LearnerDuoContext db)
        {
            if (await db.Users.AnyAsync()) return;

            try
            {
                var userData = await File.ReadAllTextAsync("Data/UserSeedData.json");
                var users = JsonSerializer.Deserialize<List<User>>(userData);
                foreach (var user in users)
                {
                    using var hmac = new HMACSHA512();


                    user.UserName = user.UserName.ToLower();
                    user.PasswordHash = hmac.ComputeHash(Encoding.UTF8.GetBytes("012012"));
                    user.PasswordSalt = hmac.Key;

                    db.Users.Add(user);
                }
                await db.SaveChangesAsync();

                Console.WriteLine("Add success");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message.ToString());
            }
            
        }
    }
}
