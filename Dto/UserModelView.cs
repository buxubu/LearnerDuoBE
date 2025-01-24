using LearnerDuo.Dto;
using LearnerDuo.Extentions;
using LearnerDuo.Models;

namespace LearnerDuo.ModelViews
{
    public class UserModelView
    {

    }
    public class Register
    {
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string City { get; set; }
        public string Country { get; set; }
        public DateTime Birthday { get; set; }
        public string Gender { get; set; }
        public string KnownAs { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        
    }

    public class Login
    {
        public string UserName { get; set; }
        public string Password { get; set; }
    }
    public class UserToken
    {
        public string UserName { get; set; }
        public string Token { get; set; }
        public string PhotoUrl { get; set; }
        public string KnownAs { get; set; }
        public string Gender { get; set; }
    }

    public class MemberDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? PhotoUrl {  get; set; }
        public string? City { get; set; }
        public string? Country { get; set; }
        public DateTime? Created { get; set; }
        public int Age { get; set; }
        public string? Gender { get; set; }
        public string? LookingFor { get; set; }
        public string? Interests { get; set; }
        public string? KnownAs { get; set; }
        public string? Description { get; set; }
        public DateTime? LastActive { get; set; } 
        public string? Status { get; set; }
        public int? Coin { get; set; }
        public int? Price { get; set; }

        public virtual ICollection<PhotoDto>? Photos { get; set; }


    }

    public class UserParams
    {
        private const int MaxPageSize = 50;
        public int PageNumber { get; set; } = 1;   
        private int pageSize = 10;

        public int PageSize
        {
            get => pageSize;
            set => pageSize = (value > MaxPageSize) ? MaxPageSize : value;
        }
        public string Gender { get; set; }
        public int MinAge { get; set; } = 18;
        public int MaxAge { get; set; } = 150;
        public string OrderBy { get; set; } = "lastActive";
    }
}
