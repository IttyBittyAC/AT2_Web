namespace MVCApplication.Models.Config
{
    /// Singleton class to represent footer configuration loaded from XML configuration
    public class Footer
    {
        public List<Column> Columns { get; set; } = new List<Column>();
    }

    // Class to represent a column in the footer, which contains a title and a list of links
    public class Column
    {
        public string Title { get; set; } = string.Empty;
        public List<Link> Links { get; set; } = new List<Link>();
    }

    // Class to represent a link in the footer, which contains display text and a URL
    public class Link
    {
        public string Text { get; set; } = string.Empty;
        public string Url { get; set; } = string.Empty;
    }
}