namespace MVCApplication.Models
{
    public class AndInTheDarknessBindThem
    {
        // Route
        public string Table { get; set; } = string.Empty;
        public int? Id { get; set; } = null;

        // State
        public string? Title { get; set; } = null;
        public string? Error { get; set; } = null;
        public string? Success { get; set; } = null;
        public string? ReturnUrl { get; set; } = null;

        // User
        public string? CurrentUser { get; set; } = null; 
        public string? CurrentRole { get; set; } = null;
        public bool IsAdmin => CurrentRole == "Admin";
        public bool IsAuth => CurrentUser is not null;

        // Check Table
        public string FormAction => Id.HasValue ? $"/{Table}/{Id}" : $"/{Table}";

        // Models
        public Booking? Booking { get; set; }
        public List<Booking> Bookings { get; set; } = [];
        public Event? Event { get; set; }
        public List<Event> Events { get; set; } = [];
        public Feedback? Feedback { get; set; }
        public List<Feedback> Feedbacks { get; set; } = [];
        public User? User { get; set; }
        public List<User> Users { get; set; } = [];

        public static AndInTheDarknessBindThem Build(string table, string? currentUser, string? currentRole, string? title = null, string? returnurl = null) =>
            new AndInTheDarknessBindThem
            {
                Table = table,
                Title = title ?? table,
                CurrentUser = currentUser,
                CurrentRole = currentRole,
                ReturnUrl = returnurl
            };
    }
}
