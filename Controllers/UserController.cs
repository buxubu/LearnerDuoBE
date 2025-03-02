using Azure.Core;
using LearnerDuo.Dto;
using LearnerDuo.Models;
using LearnerDuo.ModelViews;
using LearnerDuo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Serialization;
using Newtonsoft.Json;
using System.ComponentModel.Design.Serialization;
using System.Diagnostics;
using System.Security.Cryptography;
using System.Text;
using LearnerDuo.Extentions;

namespace LearnerDuo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : BaseAPIController
    {
        private readonly IUserService _userService;
        public UserController(IUserService userService)
        {
            _userService = userService;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("register")]
        public async Task<IActionResult> Register(Register model)
        {
            try
            {
                var user = await _userService.Register(model);
                if (user == null) return BadRequest("User is used. ");
                return Ok(user);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.ToString());
            }
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("login")]
        public async Task<IActionResult> Login(Login model)
        {
            UserToken userDto = await _userService.Login(model);
            if (userDto == null) return BadRequest("Invalid username or password");
            return Ok(userDto);
        }

        //[Authorize(Roles = "Admin")]
        [HttpGet]
        [Route("getUsers")]
        public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)
        {
            try
            {
                // we can use this to get opposite gender but we already handle this in the client side

                //var userGender = _userService.GetGenderUser(User.GetUserName());

                //if (!string.IsNullOrEmpty(userParams.Gender)) userParams.Gender = userGender.ToString() == "male" ? "female" : "male";
                // receive value from PageList<T> we created recently a class to handle the pagination
                var users = await _userService.GetUsers(userParams);

                // add the pagination to the header
                Response.AddPaginationHeader(users.CurrentPage, users.PageSize, users.TotalCount, users.TotalPages);

                return Ok(users);
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.ToString());
            }
        }


        [HttpGet]
        [Route("detail/{name}")]
        public async Task<IActionResult> Detail(string name)
        {
            try
            {
                return Ok(await _userService.Detail(name));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.ToString());
            }
        }

        [Authorize(Roles = "Member")]
        [HttpGet]
        [Route("findUser/{userName}")]
        public async Task<IActionResult> FindUser(string userName)
        {
            try
            {
                return Ok(await _userService.FindUser(userName));
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.ToString());
            }
        }

        [HttpPut("editUser")]
        public async Task<IActionResult> EditUser(MemberDto model)
        {
            try
            {
                await _userService.EditUser(model);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, ex.Message.ToString());
            }
        }

        [HttpPost("addPhoto")]
        public async Task<ActionResult<ActionResult<PhotoDto>>> AddPhoto(IFormFile file)
        {
            try
            {
                var photoDto = await _userService.AddPhoto(file);

                if (photoDto == null) return BadRequest("Add photo to cloudinary is error, please check again. ");

                //// the new way if u want return with Json
                //if (!result.Success) return BadRequest(result.Result);

                //var serializerSettings = new JsonSerializerSettings
                //{
                //    ContractResolver = new CamelCasePropertyNamesContractResolver(),
                //    NullValueHandling = NullValueHandling.Ignore,
                //    Formatting = Formatting.Indented
                //};
                //var jsonResult = new JsonResult(result) { ContentType = "application/json", SerializerSettings = serializerSettings, StatusCode = 200 };
                //return Ok(jsonResult.Value);

                // return with type Text and you should convert to json type in client
                return Ok(photoDto);
            }

            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("setMainPhoto/{photoId:int}")]
        public async Task<IActionResult> SetMainPhoto(int photoId)
        {
            try
            {
                var result = await _userService.SetMainPhoto(photoId);
                if (result == null) return BadRequest(result.Result);
                return Ok(new { result.Result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpDelete("deletePhoto/{photoId:int}")]
        public async Task<IActionResult> DeletPhoto(int photoId)
        {
            try
            {
                var result = await _userService.DeletePhoto(photoId);

                if (!result.Success)
                {
                    return BadRequest(result.Result);
                }

                return Ok(new { result.Result });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }

}


