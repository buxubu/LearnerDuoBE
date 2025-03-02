using LearnerDuo.Data;
using LearnerDuo.Dto;
using LearnerDuo.Extentions;
using LearnerDuo.Helper;
using LearnerDuo.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnerDuo.Services
{
    public interface ILikesService
    {
        Task<UserLike> GetUserLike(int sourceUserId, int likedUserId);
        Task<User> GetUserWithLikes(int userId);
        Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams);
        Task<NotificationResults> AddLike(string userName);
    }

    public class LikesService : ILikesService
    {
        private readonly LearnerDuoContext _db;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public LikesService(LearnerDuoContext db, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _db = db;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        // this is just a function check if user like or not
        public async Task<UserLike> GetUserLike(int sourceUserId, int likedUserId)
        {
            return await _db.Likes.FindAsync(sourceUserId, likedUserId);
        }

        // this is just a function find user like FindUserId in UserService
        public async Task<User> GetUserWithLikes(int userId)
        {
            return await _db.Users
                .Include(x => x.LikedUsers)
                .FirstOrDefaultAsync(x => x.Id == userId);
        }

        public async Task<PagedList<LikeDto>> GetUserLikes(LikeParams likeParams)
        {
            var users = _db.Users.OrderBy(u => u.UserName).AsQueryable();
            var likes = _db.Likes.AsQueryable();

            if (likeParams.Predicate == "liked")
            {
                likes = likes.Where(like => like.SourceUserId == likeParams.UserId); // find out how many time you liked
                users = likes.Select(like => like.LikedUser); // from ablove find out who you liked
            }

            if (likeParams.Predicate == "likedBy")
            {
                likes = likes.Where(like => like.LikedUserId == likeParams.UserId);// find out who liked you in Db UserLike    
                users = likes.Select(like => like.SourceUser); // from ablove find out who liked you
            }

            return await PagedList<LikeDto>.CreateAsync(users.Select(user => new LikeDto
            {
                UserId = user.Id,
                Age = user.Birthday.CaculateAge(),
                UserName = user.UserName,
                KnownAs = user.KnownAs,
                City = user.City,
                PhotoUrl = user.Photos.FirstOrDefault(p => p.IsMain).Url
            }), likeParams.PageNumber, likeParams.PageSize);
        }

        public async Task<NotificationResults> AddLike(string userName)
        {
            var sourceUserId = _httpContextAccessor.HttpContext.User.GetUserId(); // Get id of the person currently logged in 
            var likedUser = await _userService.FindUser(userName); // Get the user who you want to like
            // this is just a function find user like FindUserId in UserService
            var sourceUser = await GetUserWithLikes(sourceUserId);// Get user liked

            if (likedUser == null) return new NotificationResults { Success = false, StatusCode = 404, Result = "NotFound" };

            if (sourceUser.UserName == userName) return new NotificationResults { Success = false, StatusCode = 400, Result = "You cannot like yourself" };

            var userLike = await GetUserLike(sourceUserId, likedUser.Id);// check and get both source user and liked

            if (userLike != null)
            {
                return new NotificationResults { Success = false, StatusCode = 400, Result = "You already liked this user" };
            }

            userLike = new UserLike
            {
                SourceUserId = sourceUserId,
                LikedUserId = likedUser.Id,
            };

            sourceUser.LikedUsers.Add(userLike); // that way like _db.likes.Add(userLike)
            await _db.SaveChangesAsync();
            return new NotificationResults { Success = true, StatusCode = 200, Result = "You have liked " + likedUser.UserName };
        }

    }
}
