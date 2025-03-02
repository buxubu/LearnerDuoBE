namespace LearnerDuo.Dto
{
    public class NotificationResults
    {
        public bool Success { get; set; }
        public int? StatusCode { get; set; }
        public string Result { get; set; }
        public object Data {  get; set; }
    }

    public class NotificationResults<T>
    {
        public bool Success { get; set; }
        public int? StatusCode { get; set; }
        public string Result { get; set; } 
        public T Data { get; set; }
    }

}
