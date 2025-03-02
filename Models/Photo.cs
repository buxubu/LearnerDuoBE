using System.ComponentModel.DataAnnotations.Schema;

namespace LearnerDuo.Models
{
    [Table("Photo")]
    public class Photo
    {
        public int PhotoId { get; set; }
        public string Url { get; set; }
        public bool IsMain {  get; set; }   
        public string PublicId { get;set; }
        public int? UserId { get; set; }
        public virtual User User { get; set; }


    }
}
