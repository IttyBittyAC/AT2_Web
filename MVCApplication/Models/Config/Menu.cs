namespace MVCApplication.Models.Config
{
    /// Class to represent a menu item, which contains display text, a URL, and an order for sorting
    public class MenuItem
    {
        public string Text { get; set; } = string.Empty; // Display text for the menu item
        public string Url { get; set; } = string.Empty; // URL the menu item points to
        public int Order { get; set; } // Order of the menu item
    }

    // Singleton class to represent a menu loaded from XML configuration, which contains an ID and a list of menu items
    public class Menu
    {
        public string Id { get; set; } = string.Empty; // Unique identifier for the menu
        public List<MenuItem> Items { get; set; } = new List<MenuItem>(); // List of menu items

    }
}
