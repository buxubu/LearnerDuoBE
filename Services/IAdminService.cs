using LearnerDuo.Dto;
using LearnerDuo.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LearnerDuo.Services
{
    public interface IAdminService
    {
        Task<IEnumerable<object>> GetUsersWithRoles();
        Task<NotificationResults> EditUserRoles(string userName, [FromQuery] string roles);
    }

    public class AdminService : IAdminService
    {
        private readonly UserManager<User> _userManager;
        public AdminService(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<NotificationResults> EditUserRoles(string userName, [FromQuery] string roles)
        {
            var selectedRoles = roles.Split(',').ToArray(); // split array to add to array again

            if (userName == null || selectedRoles.Length == 0) return new NotificationResults { Success = false, StatusCode = 404, Result = "Please check again username or roles" };

            var user = await _userManager.FindByNameAsync(userName);

            var userRoles = await _userManager.GetRolesAsync(user);

            // explain it mean add selectedRoles when user pass data but except default role we found in database
            var result = await _userManager.AddToRolesAsync(user, selectedRoles.Except(userRoles));
            if (!result.Succeeded) return new NotificationResults { Success = false, StatusCode = 400, Result = "Failed to add to roles. " };

            // explain it mean remove default data in databasae but except the new selectedRoles we just save
            result = await _userManager.RemoveFromRolesAsync(user, userRoles.Except(selectedRoles));
            if (!result.Succeeded) return new NotificationResults { Success = false, StatusCode = 400, Result = "Failed to remove default role. " };

            return new NotificationResults
            {
                Success = true,
                StatusCode = 200,
                Result = "Success edit roles. ",
                Data = await _userManager.GetRolesAsync(user)
            };
        }

        async Task<IEnumerable<object>> IAdminService.GetUsersWithRoles()
        {
            return await _userManager.Users
                                     .Include(ur => ur.UserRoles)
                                     .ThenInclude(r => r.Role)
                                     .OrderBy(o => o.UserName)
                                     .Select(x => new
                                     {
                                         x.Id,
                                         UserName = x.UserName,
                                         Roles = x.UserRoles.Select(r => r.Role.Name).ToList()
                                     })
                                     .ToListAsync();
        }
    }
}
