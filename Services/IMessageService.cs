using AutoMapper;
using AutoMapper.QueryableExtensions;
using LearnerDuo.Data;
using LearnerDuo.Dto;
using LearnerDuo.Extentions;
using LearnerDuo.Helper;
using LearnerDuo.Models;
using Microsoft.EntityFrameworkCore;

namespace LearnerDuo.Services
{
    public interface IMessageService
    {
        void AddGroup(Group group);
        void RemoveConnection(Connection connection);
        Task<Connection> GetConnection(string connectionId);
        Task<Group> GetMessageGroup(string groupName);
        Task<Group> GetGroupForConnection(string connectionId);

        Task<NotificationResults<MessageDto>> CreateMessage(CreateMessageDto createMessageDto);
        //void DeleteMessage(Message message);
        void DeleteMessage(int messageId);
        void UpdateMessage(Message message);
        Task<Message> GetMessage(int messageId);
        Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams);
        Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername);
        Task<bool> SaveAllAsync();
    }

    public class MessageService : IMessageService
    {
        private readonly LearnerDuoContext _db;
        private readonly IMapper _mapper;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IUserService _userService;
        public MessageService(LearnerDuoContext db, IMapper mapper, IHttpContextAccessor httpContextAccessor, IUserService userService)
        {
            _db = db;
            _httpContextAccessor = httpContextAccessor;
            _mapper = mapper;
            _userService = userService;
        }

        public void AddGroup(Group group)
        {
            _db.Groups.Add(group);
        }

        public async Task<Connection> GetConnection(string connectionId)
        {
            return await _db.Connections.FindAsync(connectionId);
        }

        public async Task<Group> GetMessageGroup(string groupName)
        {
            return await _db.Groups.Include(x => x.Connections).FirstOrDefaultAsync(x => x.Name == groupName);
        }

        public void RemoveConnection(Connection connection)
        {
            _db.Connections.Remove(connection);
        }

        public async Task<Group> GetGroupForConnection(string connectionId)
        {
            return await _db.Groups.Include(x => x.Connections)
                                   .Where(x => x.Connections
                                   .Any(x => x.ConnectionId == connectionId))
                                   .FirstOrDefaultAsync();
        }

        public async Task<NotificationResults<MessageDto>> CreateMessage(CreateMessageDto createMessageDto)
        {
            var userName = _httpContextAccessor.HttpContext.User.GetUserName();
            if (userName == createMessageDto.RecipientUsername.ToLower()) return new NotificationResults<MessageDto> { Success = false, StatusCode = 400, Result = "You cannot send messages to yourself.", Data = null };

            var sender = await _db.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == userName);
            var recipient = await _db.Users.Include(x => x.Photos).FirstOrDefaultAsync(x => x.UserName == createMessageDto.RecipientUsername);
            if (recipient == null) return new NotificationResults<MessageDto> { Success = false, StatusCode = 404, Result = "Recipient not found.", Data = null };

            var message = new Message
            {
                Sender = sender,
                Recipient = recipient,
                SenderUsername = sender.UserName,
                RecipientUsername = recipient.UserName,
                Content = createMessageDto.Content,
                MessageSent = DateTime.Now,
            };

            var mapMessageDto = _mapper.Map<MessageDto>(message);
            mapMessageDto.SenderKnownAs = sender.KnownAs;
            mapMessageDto.RecipientKnownAs = recipient.KnownAs;

            await _db.Messages.AddAsync(message);
            await _db.SaveChangesAsync();
            return new NotificationResults<MessageDto> { Success = true, StatusCode = 200, Result = "Message sent successfully.", Data = mapMessageDto };
        }

        public  void DeleteMessage(int messageId)
        {
            _db.Messages.Remove(_db.Messages.Find(messageId));
            _db.SaveChanges();
        }

        

        public async Task<Message> GetMessage(int messageId)
        {
            return await _db.Messages
                         .Include(x=>x.Recipient)
                         .Include(x=>x.Sender)
                         .SingleOrDefaultAsync(x=>x.MessageId ==messageId);
        }

      

        public async Task<PagedList<MessageDto>> GetMessagesForUser(MessageParams messageParams)
        {
            var query = _db.Messages.OrderByDescending(x => x.MessageSent)
                                    //when we use this itt mean we map the message to messageDto after we find messages with condition above
                                    // optimize queries
                                    .ProjectTo<MessageDto>(_mapper.ConfigurationProvider)
                                    .AsQueryable();

            query = messageParams.Container switch
            {
                // and when we use projectTo we just need to Straight access in MessageDto to get the data
                // inbox to another user, and if you delete the message, it will not show ,
                // of cause it won't delete message it just hidden and it'll be delete when both user delete the message
                "Inbox" => query.Where(x => x.RecipientUsername == messageParams.Username 
                                       && x.RecipientDeleted == false),
                "Outbox" => query.Where(x => x.SenderUsername == messageParams.Username 
                                       && x.SenderDeleted == false), // outbox from the user
                _ => query.Where(x => x.RecipientUsername == messageParams.Username 
                                       && x.DateRead == null 
                                       && x.RecipientDeleted == false)
            };

            // if not use this way you need to do that below
            // var messages = query.ToList();
            // var mappedMessages = _mapper.Map<List<MessageDto>>(messages);


            //var messages = query.ProjectTo<MessageDto>(_mapper.ConfigurationProvider);

            return await PagedList<MessageDto>.CreateAsync(query, messageParams.PageNumber, messageParams.PageSize);
        }

        public async Task<IEnumerable<MessageDto>> GetMessageThread(string currentUsername, string recipientUsername)
        {
            var messages = await _db.Messages
                //we use this unclude just 1 purpose is gte the photo of user,
                //and beacause we use projectTo below so we don't need to use this
                        //.Include(u => u.Sender).ThenInclude(p => p.Photos)
                        //.Include(u => u.Recipient).ThenInclude(p => p.Photos)
                        .Where(x => x.Recipient.UserName == currentUsername 
                               && x.Sender.UserName == recipientUsername
                               && x.RecipientDeleted == false
                               || x.Recipient.UserName == recipientUsername 
                               && x.Sender.UserName == currentUsername
                               && x.SenderDeleted == false)
                        .OrderBy(x => x.MessageSent)
                        //when we use this itt mean we map the message to messageDto after we find messages with condition above
                        // optimize queries
                        .ProjectTo<MessageDto>(_mapper.ConfigurationProvider) 
                        .ToListAsync(); // get all messages between two users

            // if the message is not read, then update the message to read
            var unreadMessages = messages.Where(x => x.DateRead == null && x.RecipientUsername == currentUsername).ToList(); 

            if (unreadMessages.Any())
            {
                foreach (var message in unreadMessages)
                {
                    message.DateRead = DateTime.Now;
                }

                await _db.SaveChangesAsync();
            }
            return messages;
        }

        

        public async Task<bool> SaveAllAsync()
        {
            return await _db.SaveChangesAsync() > 0;
        }

        public void UpdateMessage(Message message)
        {
            _db.Update(message);
        }

       
    }
}
