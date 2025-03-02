using Microsoft.AspNetCore.Identity;

namespace LearnerDuo.Models
{
    public class Role : IdentityRole<int>
    {
        public virtual ICollection<UserRole> UserRoles { get; set; }
    }
}
