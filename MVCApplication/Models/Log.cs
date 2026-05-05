namespace MVCApplication.Models
{
    public class Log
    {
        public int Id { get; set; }
        public bool IsError { get; set; }
        public string UserName { get; set; } = string.Empty;
        public string Role { get; set; } = string.Empty;
        public string View { get; set; } = string.Empty;
        public string Message { get; set; } = string.Empty;
        public DateTime DateTime { get; set; }
    }
}
