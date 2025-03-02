using LearnerDuo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnerDuo.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AdminController : BaseAPIController
    {
        private readonly IAdminService _adminService;
        public AdminController(IAdminService adminService)
        {
            _adminService = adminService;
        }

        [Authorize(Policy = "RequireAdminRole")]
        [HttpGet("users-with-roles")]
        public async Task<IActionResult> GetUsersWithRoles()
        {
            return Ok(await _adminService.GetUsersWithRoles());
        }

        [HttpPut("edit-roles/{username}")]
        public async Task<IActionResult> EditUserRoles(string username, [FromQuery] string roles)
        {
            var resultEditRoles = await _adminService.EditUserRoles(username, roles);

            if (!resultEditRoles.Success)
            {
                switch (resultEditRoles.StatusCode)
                {
                    case 400:
                        return BadRequest(resultEditRoles.Result);
                    case 404:
                        return NotFound(resultEditRoles.Result);
                    default:
                        break;
                }
            }
            return Ok(new { resultEditRoles.Data });
        }

        [Authorize(Policy = "ModeratePhotoRole")]
        [HttpGet("photo-to-moderate")]
        public IActionResult GetPhotosForModeration()
        {
            return Ok("Adminn or Moderate can see this. ");
        }

    }
}
