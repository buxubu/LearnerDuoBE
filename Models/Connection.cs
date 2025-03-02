namespace LearnerDuo.Models
{
    public class Connection
    {
        public Connection() // 1 contructor use with purpose none request
        {
        }
        public Connection(string connectionId, string username) // 1 contructor use with purpose request 2 parameter
        {
            ConnectionId = connectionId;
            UserName = username;
        }
        public string ConnectionId { get; set; }
        public string UserName { get; set; }
    }
}