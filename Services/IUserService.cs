using AutoMapper;
using AutoMapper.QueryableExtensions;
using LearnerDuo.Data;
using LearnerDuo.Dto;
using LearnerDuo.Extentions;
using LearnerDuo.Helper;
using LearnerDuo.Models;
using LearnerDuo.ModelViews;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion.Internal;
using System.Reflection.Emit;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;

namespace LearnerDuo.Services
{
    public interface IUserService
    {
        Task<UserToken> Login(Login model);
        Task<UserToken> Register(Register model);
        Task<PagedList<MemberDto>> GetUsers(UserParams userParams);
        Task<string> GetGenderUser(string userName);
        Task<MemberDto> Detail(string name);
        Task<MemberDto> FindUser(string userName);
        Task<MemberDto> FindUserId(int userId);
        Task EditUser(MemberDto model);
        Task<PhotoDto> AddPhoto(IFormFile file);
        Task<NotificationResults> SetMainPhoto(int photoId);
        Task<NotificationResults> DeletePhoto(int photoId);
        Task UpdateUserDb(MemberDto user);
        Task<bool> Save();
    }

    public class UserService : IUserService
    {
        private readonly LearnerDuoContext _db;
        private readonly ITokenService _tokenService;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IPhotoService _photoService;
        private readonly UserManager<User> _userManager;
        private readonly SignInManager<User> _signInManager;

        public UserService(ITokenService tokenService, IMapper mapper, IHttpContextAccessor httpContextAccessor,
                           IPhotoService photoService, UserManager<User> userManager, SignInManager<User> signInManager,
                           LearnerDuoContext db)
        {
            _tokenService = tokenService;
            _mapper = mapper;
            _httpContextAccessor = httpContextAccessor;
            _photoService = photoService;
            _userManager = userManager;
            _signInManager = signInManager;
            _db = db;
        }

        public async Task<UserToken> Login(Login model)
        {
            var user = await _userManager.Users.Include(x => x.Photos)
                                       .AsNoTracking()
                                       .SingleOrDefaultAsync(x => x.UserName == model.UserName);

            if (user == null) return null;

            var result = await _signInManager.CheckPasswordSignInAsync(user, model.Password, false); // update here

            if (!result.Succeeded) return null;

            return new UserToken
            {
                UserName = user.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }


        public async Task<UserToken> Register(Register model)
        {
            if (await UserExists(model.UserName)) return null;
            using var hmac = new HMACSHA512();

            var user = _mapper.Map<User>(model);

            user.UserName = model.UserName.ToLower();

            var result = await _userManager.CreateAsync(user, model.Password); // when use UserManager, we don't need to Add or SaveChanges
            if (!result.Succeeded) return null;

            var addUserRole = await _userManager.AddToRoleAsync(user, "Member");
            if (addUserRole == null) return null;

            return new UserToken
            {
                UserName = model.UserName,
                Token = await _tokenService.CreateToken(user),
                PhotoUrl = user.Photos.FirstOrDefault(x => x.IsMain == true)?.Url,
                KnownAs = user.KnownAs,
                Gender = user.Gender
            };
        }


        public async Task<bool> UserExists(string username)
        {
            return await _db.Users.AnyAsync(x => x.UserName.ToLower() == username.ToLower());
        }


        public async Task<PagedList<MemberDto>> GetUsers(UserParams userParams)
        {
            var getUserName = _httpContextAccessor.HttpContext.User.GetUserName();


            var minDob = DateTime.Today.AddYears(-userParams.MaxAge - 1);
            var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

            var query = _db.Users.AsQueryable();

            query = query.Where(x => x.UserName != getUserName);

            query = query.Where(x => x.Gender == userParams.Gender);

            query = query.Where(x => x.Birthday >= minDob && x.Birthday <= maxDob);

            query = userParams.OrderBy switch
            {
                "created" => query.OrderByDescending(x => x.Created),
                _ => query.OrderByDescending(x => x.LastActive)
            };


            return await PagedList<MemberDto>.CreateAsync(query.ProjectTo<MemberDto>(_mapper.ConfigurationProvider).AsNoTracking(),
                                                          userParams.PageNumber, userParams.PageSize);
        }


        public async Task<MemberDto> Detail(string name)
        {
            var findUser = await _db.Users.Where(x => x.LastName.Contains(name))
                                          .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                                          .SingleOrDefaultAsync();
            return _mapper.Map<MemberDto>(findUser);
        }


        public async Task<MemberDto> FindUser(string userName)
        {
            var findUser = await _db.Users.Where(x => x.UserName.ToLower() == userName.ToLower())
                                          .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                                          .SingleOrDefaultAsync();
            return _mapper.Map<MemberDto>(findUser);
        }


        public async Task EditUser(MemberDto model)
        {
            var getUserName = _httpContextAccessor.HttpContext.User.GetUserName();
            var findUser = await _db.Users.SingleOrDefaultAsync(x => x.UserName == getUserName);

            _mapper.Map(model, findUser);

            _db.Users.Update(findUser);
            await _db.SaveChangesAsync();
        }


        public async Task<PhotoDto> AddPhoto(IFormFile file)
        {
            var getUserName = _httpContextAccessor.HttpContext.User.GetUserName();

            var user = await FindUser(getUserName);

            var result = await _photoService.AddPhotoAsync(file);

            if (result.Error != null) return null;

            var photo = new Photo
            {
                Url = result.SecureUrl.AbsoluteUri,
                PublicId = result.PublicId,
                UserId = user.Id
            };

            if (user.Photos.Count == 0)
            {
                photo.IsMain = true;
            }
            else
            {
                photo.IsMain = false;
            }

            _db.Photos.Add(photo);

            await _db.SaveChangesAsync();

            return _mapper.Map<PhotoDto>(photo);
        }

        public async Task<NotificationResults> SetMainPhoto(int photoId)
        {
            var user = await _db.Users.Include(x => x.Photos)
                                      .Where(x => x.UserName == (_httpContextAccessor.HttpContext.User.GetUserName()))
                                      .AsNoTracking()
                                      .SingleOrDefaultAsync();

            if (user == null) return new NotificationResults { Success = false, Result = "Not found user, please check your login. " };

            var photo = user.Photos.FirstOrDefault(x => x.PhotoId == photoId);

            if (photo.IsMain == true) return new NotificationResults { Success = false, Result = "This images already main photo. " };

            var currentPhoto = user.Photos.FirstOrDefault(x => x.IsMain == true);

            if (currentPhoto == null) return new NotificationResults { Success = false, Result = "This current photo is null." };

            currentPhoto.IsMain = false;

            photo.IsMain = true;

            _db.Photos.Update(photo);

            await _db.SaveChangesAsync();

            return new NotificationResults { Success = true, Result = "Set photo main success. " };


        }

        public async Task<NotificationResults> DeletePhoto(int photoId)
        {
            var user = await _db.Users.Include(x => x.Photos)
                                      .Where(x => x.UserName == _httpContextAccessor.HttpContext.User.GetUserName())
                                      .AsNoTracking()
                                      .SingleOrDefaultAsync();                  

            if (user == null) return new NotificationResults { Success = false, Result = "Not found user, please check your login. " };

            var photo = user.Photos.FirstOrDefault(x => x.PhotoId == photoId);

            if (photo == null) return new NotificationResults { Success = false, Result = "Photo not found in database. " };

            if (photo.IsMain == true) return new NotificationResults { Success = false, Result = "You can't remove the main photo. " };

            if (photo.PublicId == null) return new NotificationResults { Success = false, Result = "PulicId of photo not found. " };

            var resultDeletePhoto = await _photoService.DeletePhotoAsync(photo.PublicId);

            if (resultDeletePhoto.Error != null) return new NotificationResults { Success = false, Result = resultDeletePhoto.Error.Message };

            _db.Photos.Remove(photo);
            await _db.SaveChangesAsync();

            return new NotificationResults { Success = true, Result = "Delete photo success" };


        }


        public async Task UpdateUserDb(MemberDto memberDto)
        {
            var users = await _db.Users.FindAsync(memberDto.Id);

            users.LastActive = memberDto.LastActive.Value;

            _db.Update(users);
            await _db.SaveChangesAsync();
        }

        public async Task<bool> Save()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public async Task<MemberDto> FindUserId(int userId)
        {
            return await _db.Users.Where(x => x.Id == userId)
                            .ProjectTo<MemberDto>(_mapper.ConfigurationProvider)
                            .SingleOrDefaultAsync();
        }

        public async Task<string> GetGenderUser(string userName)
        {
            return await _db.Users.Where(x => x.UserName == userName).Select(g=>g.Gender).FirstOrDefaultAsync();
        }
    }
}
