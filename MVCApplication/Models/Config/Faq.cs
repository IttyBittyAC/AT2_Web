namespace MVCApplication.Models.Config
{
    //Singleton class to represent FAQ items loaded from XML configuration
    public class Faq
    {
        public string Id { get; set; } = string.Empty;
        public string Question { get; set; } = string.Empty;
        public string Answer { get; set; } = string.Empty;
        public int Order { get; set; } = 0;
    }
}
