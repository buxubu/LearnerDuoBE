namespace LearnerDuo.Dto
{
    public class LikeModelView
    {
    }

    public class LikeDto
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string KnownAs { get; set; }
        public string City { get; set; }
        public int Age { get; set; }
        public string PhotoUrl { get; set; }
    }

    public class LikeParams: PaginationParams
    {
        public int UserId { get; set; }
        public string Predicate { get; set; }
    }

}
