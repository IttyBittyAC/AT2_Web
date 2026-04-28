namespace MVCApplication.Models
{
    public class Log
    {
        public int ?Id { get;}
        public bool IsError { get; set; }
        public string Message { get; set; }
        public string View {  get; set; }
        public string UserName { get; set;}
        public string UserPassword { get; set; }
        public DateTime DateTime { get; set; }

    }
}
