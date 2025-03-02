using LearnerDuo.Extentions;
using Microsoft.AspNetCore.Identity;

namespace LearnerDuo.Models
{
    public class User : IdentityUser<int>
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime Created { get; set; } = DateTime.Now;
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string LookingFor { get; set; }
        public string Interests { get; set; }
        public string KnownAs { get; set; }
        public string Description { get; set; }
        public DateTime LastActive { get; set; } = DateTime.Now;
        public string Status { get; set; }
        public int Coin { get; set; }
        public int Price { get; set; }

        public virtual ICollection<Photo> Photos { get; set; }
        public virtual ICollection<UserLike> LikedByUsers { get; set; }
        public virtual ICollection<UserLike> LikedUsers { get; set; }

        public virtual ICollection<Message> MessagesSent { get; set; }
        public virtual ICollection<Message> MessagesReceived { get; set; }

        public virtual ICollection<UserRole> UserRoles { get; set; }



    }
}
