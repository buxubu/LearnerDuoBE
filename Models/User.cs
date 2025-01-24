using LearnerDuo.Extentions;

namespace LearnerDuo.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public byte[] PasswordHash { get; set; }
        public byte[] PasswordSalt { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime? Created { get; set; } = DateTime.Now;
        public DateTime? Birthday { get; set; }
        public string? Gender { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
        public string? KnownAs { get; set; }
        public string? Description { get; set; }
        public DateTime? LastActive { get; set; } = DateTime.Now;
        public string? Status { get; set; }
        public int? Coin { get; set; }
        public int? Price { get; set; }

        public virtual ICollection<Photo>? Photos { get; set; }

        //public int GetAge()
        //{
        //    return Birthday.Value.CaculateAge();
        //}

    }
}
