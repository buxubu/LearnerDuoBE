using System.ComponentModel.DataAnnotations;

namespace LearnerDuo.Models
{
    public class Group
    {
        public Group() // 1 contructor use with purpose none request
        {
        }
        public Group(string name) // 1 contructor use with purpose request 2 parameter
        {
            Name = name;
        }
        [Key]
        public string Name { get;set; }
        public virtual ICollection<Connection> Connections { get; set; }  = new List<Connection>();
    }
}
