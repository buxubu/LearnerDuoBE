using LearnerDuo.Models;

namespace LearnerDuo.Dto
{
    public class PhotoModelView
    {
    }
    public class PhotoDto
    {
        public int PhotoId { get; set; }
        public string? Url { get; set; }
        public bool IsMain { get; set; }
    }
}
