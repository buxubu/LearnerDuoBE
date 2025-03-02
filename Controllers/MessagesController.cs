using Azure.Core;
using CloudinaryDotNet.Actions;
using LearnerDuo.Dto;
using LearnerDuo.Extentions;
using LearnerDuo.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace LearnerDuo.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class MessagesController : BaseAPIController
    {
        private readonly IMessageService _messageService;
        private readonly IUserService _userService;
        public MessagesController(IMessageService messageService, IUserService userService)
        {
            _messageService = messageService;
            _userService = userService;
        }

        [HttpPost]
        public async Task<IActionResult> CreateMesaage(CreateMessageDto createMessageDto)
        {
            var resultCreateMessage = await _messageService.CreateMessage(createMessageDto);

            if (!resultCreateMessage.Success)
            {
                switch (resultCreateMessage.StatusCode)
                {
                    case 400:
                        return BadRequest(resultCreateMessage.Result);
                    case 404:
                        return NotFound(resultCreateMessage.Result);
                    default:
                        return BadRequest("Failed to sent message. ");
                }
            }

            return Ok(resultCreateMessage.Data);
        }

        [HttpGet]
        public async Task<IActionResult> GetMessagesForUser([FromQuery] MessageParams messageParams)
        {
            messageParams.Username = User.GetUserName();

            var messages = await _messageService.GetMessagesForUser(messageParams);
            Response.AddPaginationHeader(messages.CurrentPage, messages.PageSize, messages.TotalCount, messages.TotalPages);

            return Ok(messages);
        }

        // this function when you click component and read the message
        [HttpGet("thread/{recipientUsername}")]
        public async Task<IActionResult> GetMessageThread(string recipientUsername)
        {
            var currentUsername = User.GetUserName();

            var messages = await _messageService.GetMessageThread(currentUsername, recipientUsername);

            return Ok(messages);
        }

        [HttpDelete("{messageId}")]
        public async Task<IActionResult> DeleteMessage(int messageId)
        {
            var userName = User.GetUserName();

            var getMessage = await _messageService.GetMessage(messageId);

            if (getMessage != null && userName != null)
            {
                if (getMessage.Sender.UserName != userName && getMessage.Recipient.UserName != userName)
                    return Unauthorized();

                // if you are the sender, and you want to delete the message, set the SenderDeleted to true and hidden message
                if (getMessage.Sender.UserName == userName) getMessage.SenderDeleted = true;

                // if you are the recipient, and you want to delete the message, set the RecipientDeleted to true and hidden message
                if (getMessage.Recipient.UserName == userName) getMessage.RecipientDeleted = true;

                // in case if both want to deltete the message, delete the message
                if (getMessage.SenderDeleted && getMessage.RecipientDeleted) _messageService.DeleteMessage(messageId);

                _messageService.UpdateMessage(getMessage);
            }
            return Ok();
        }
    }
}
