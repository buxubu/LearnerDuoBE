using AutoMapper;
using LearnerDuo.Data;
using LearnerDuo.Dto;
using LearnerDuo.Extentions;
using LearnerDuo.Models;
using LearnerDuo.Services;
using LearnerDuo.SignalIR;
using Microsoft.AspNetCore.SignalR;

namespace LearnerDuo.SignalR
{
    public class MessageHub : Hub
    {
        private readonly IMessageService _messageServie;
        private readonly IMapper _mapper;
        private readonly IHubContext<PresenceHub> _presence;
        private readonly PresenceTracker _presenceTracker;
        private readonly LearnerDuoContext _db;
        public MessageHub(IMessageService messageService, IMapper mapper, IHubContext<PresenceHub> presence,
                          PresenceTracker presenceTracker, LearnerDuoContext db)
        {
            _mapper = mapper;
            _messageServie = messageService;
            _presence = presence;
            _presenceTracker = presenceTracker;
            _db = db;
        }

        public override async Task OnConnectedAsync()
        {
            var httpContext = Context.GetHttpContext();
            // get user when we tranmit query like this in client side
            //      .withUrl(this.hubUrl + 'message?user=' + otherUsername, {
            //      accessTokenFactory: () => user.token,
            //})
            var otherUser = httpContext.Request.Query["user"].ToString();
            var gruopName = GetGroupName(Context.User.GetUserName(), otherUser);
            await Groups.AddToGroupAsync(Context.ConnectionId, gruopName);
            var group = await AddToGroup(gruopName); // add to group with data save in database
            await Clients.Group(gruopName).SendAsync("UpdatedGroup", group); // send information of group to client

            var messages = await _messageServie.GetMessageThread(Context.User.GetUserName(), otherUser);

            await Clients.Caller.SendAsync("ReceiveMessageThread", messages);
        }

        public override async Task OnDisconnectedAsync(Exception exception)
        {
            var group = await RemoveFromGroup(); // remove from group (connection) with data save in database
            await Clients.Group(group.Name).SendAsync("UpdatedGroup", group);

            await base.OnDisconnectedAsync(exception);
        }

        public async Task SendMessage(CreateMessageDto createMessageDto)
        {
            var resultCreateMessage = await _messageServie.CreateMessage(createMessageDto);

            if (!resultCreateMessage.Success)
            {
                switch (resultCreateMessage.StatusCode)
                {
                    case 400:
                        throw new HubException(resultCreateMessage.Result);
                    case 404:
                        throw new HubException(resultCreateMessage.Result);
                    default:
                        throw new HubException("Failed to sent message. ");
                }
            }

            var groupName = GetGroupName(Context.User.GetUserName(), createMessageDto.RecipientUsername);

            var group = await _messageServie.GetMessageGroup(groupName);// get group with include connections to check

            // when we check Connections.Any with 1 parameter UserName equal createMessageDto.RecipientUsername
            // it mean when you use function AddToGroup you also have 1 random connectionId and 1 user name is call Sender
            // ví dụ lúc đầu tài khoản 1 tên toni nhắn thì trong List Connections sẽ có UserName là toni và so sánh với người nhận tin nhắn mình gửi có tên là reva 
            // thì sẽ không thấy vì không có trong danh sách Connections và không sử dụng hàm DateRead bên dưới. Đây là tài khoản 1

            // Nếu đăng nhập bằng tài khoản 2 lúc này reva sẽ được xem là recipiennt tin nhắn lúc nảy toni gửi thì hàm OnConnectedAsync sẽ chạy và tạo thêm 1 group mới và 1 connection mới với UserName là reva
            // thì lúc này sẽ thấy trong danh sách Connections có UserName là reva và sẽ sử dụng hàm DateRead bên dưới. Đây là tài khoản 2
            //if (group.Connections.Any(x => x.UserName == createMessageDto.RecipientUsername))
            //{
            //    resultCreateMessage.Data.DateRead = DateTime.Now;
            //}
            //else
            //{
            // check users online 
            var connections = await _presenceTracker.GetConnectionsForUser(createMessageDto.RecipientUsername);
            if (connections != null) // if recipient is'nt online not send notification
            {
                // Clients(connections) mean the recipient have connections id we found above will receive notifycation 
                await _presence.Clients.Clients(connections).SendAsync("NewMessageReceived",
                new { username = resultCreateMessage.Data.SenderUsername, knownAs = resultCreateMessage.Data.SenderKnownAs });
            }
            //}
            // tracking read message and update to database
            //_db.Messages.Update(_mapper.Map<Message>(resultCreateMessage.Data));
            //await _messageServie.SaveAllAsync();
            await Clients.Group(groupName).SendAsync("NewMessage", resultCreateMessage.Data);
        }

        private async Task<Group> AddToGroup(string groupName)
        {
            // get group check
            var group = await _messageServie.GetMessageGroup(groupName);
            // this how we create another contructor in class Connection to easy to use
            var connection = new Connection(Context.ConnectionId, Context.User.GetUserName());

            if (group == null)
            {
                // this how we create another contructor in class Group to easy to use
                group = new Group(groupName);
                _messageServie.AddGroup(group); // if now already have group create new group
            }

            group.Connections.Add(connection);

            if(await _messageServie.SaveAllAsync()) return group; 

            throw new HubException("Failed to join group"); // group is ot in database
        }

        private async Task<Group> RemoveFromGroup()
        {
            var group = await _messageServie.GetGroupForConnection(Context.ConnectionId); // get group with connectionId
            var connection = group.Connections.FirstOrDefault(x => x.ConnectionId == Context.ConnectionId); // get connection with connectionId
            _messageServie.RemoveConnection(connection);

            if(await _messageServie.SaveAllAsync()) return group;

            throw new HubException("Failed to remove from group");
        }

        private string GetGroupName(string caller, string otherUser)
        {
            var stringCompare = string.CompareOrdinal(caller, otherUser) < 0;
            return stringCompare ? $"{caller}-{otherUser}" : $"{otherUser}-{caller}";
        }
    }
}
