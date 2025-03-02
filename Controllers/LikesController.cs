using LearnerDuo.Dto;
using LearnerDuo.Extentions;
using LearnerDuo.Models;
using LearnerDuo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace LearnerDuo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class LikesController : BaseAPIController
    {
        private ILikesService _likesService;
        public LikesController(ILikesService likesService)
        {
            _likesService = likesService;
        }

        [HttpPost("{userName}")]
        public async Task<IActionResult> AddLike(string userName)
        {
            var resultAddLike = await _likesService.AddLike(userName);
            if (resultAddLike.Success == false)
            {
                switch (resultAddLike.StatusCode)
                {
                    case 400:
                        return BadRequest(resultAddLike.Result);
                    case 404:
                        return NotFound();
                }
            }
            return Ok(new { resultAddLike.Result });

        }

        [HttpGet]
        public async Task<IActionResult> GetUserLikes([FromQuery] LikeParams likeParams)
        {
            likeParams.UserId = User.GetUserId();
            var userLikes = await _likesService.GetUserLikes(likeParams);

            Response.AddPaginationHeader(userLikes.CurrentPage, userLikes.PageSize, userLikes.TotalCount, userLikes.TotalPages);

            return Ok(userLikes);
        }
    }
}
