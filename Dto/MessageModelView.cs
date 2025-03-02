using LearnerDuo.Models;
using System.Text.Json.Serialization;

namespace LearnerDuo.Dto
{
    public class MessageModelView
    {

    }

    public class MessageDto
    {
        public int MessageId { get; set; }
        public int SenderId { get; set; }
        public string SenderUsername { get; set; }
        public string SenderPhotoUrl { get; set; }
        public string SenderKnownAs { get; set; }

        public int RecipientId { get; set; }
        public string RecipientUsername { get; set; }
        public string RecipientPhotoUrl { get; set; }
        public string RecipientKnownAs { get; set; }

        public string Content { get; set; }
        public DateTime? DateRead { get; set; }
        public DateTime MessageSent { get; set; }
        [JsonIgnore]
        public bool SenderDeleted { get; set; }
        [JsonIgnore]
        public bool RecipientDeleted { get; set; }
    }

    public class CreateMessageDto
    {
        public string RecipientUsername { get; set; }
        public string Content { get; set; }
    }

    public class MessageParams : PaginationParams
    {
        public string Username { get; set; } = string.Empty;
        public string Container { get; set; } = "Unread";
    }
}
