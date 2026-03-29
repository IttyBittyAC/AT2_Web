namespace MVCApplication.Models
{
    public class AndInTheDarknessBindThem
    {
        // Route
        public string Table { get; set; } = string.Empty;
        public string Action { get; set; } = string.Empty;
        public int? Id { get; set; } = null;

        // Data
        public dynamic? Row { get; set; } = null;
        public List<dynamic?> Rows { get; set; } = new List<dynamic?>();  

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

        // Checks Table
        public bool IsEdit => Row is not null && Id.HasValue;
        public bool IsCreate => Row is null;
        public bool HasRows => Rows.Count > 0;
        public string FormAction => IsEdit ? $"/{Table}/{Id}" : $"/{Table}";

        // Models
        public Booking? Booking { get; set; }
        public List<Booking> Bookings { get; set; } = new();
        public Event? Event { get; set; }
        public List<Event> Events { get; set; } = new();
        public Feedback? Feedback { get; set; }
        public List<Feedback> Feedbacks { get; set; } = new();
        public User? User { get; set; }

        public static AndInTheDarknessBindThem Build(string table, string? currentUser, string? currentRole, string? title = null, string? returnurl = null) =>
            new AndInTheDarknessBindThem
            {
                Table = table,
                Title = title ?? table,
                CurrentUser = currentUser,
                CurrentRole = currentRole,
                ReturnUrl = returnurl
            };
        //private string Get(string column)
        //{
        //    if (Row is null) return string.Empty;
        //    var obj = (IDictionary<string, object>)Row;
        //}



    }
}
