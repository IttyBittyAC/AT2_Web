namespace MVCApplication.Helpers
{
    public static class V
    {
        public static class Account
        {
            public const string Login = "~/Views/Account/Login.cshtml";
            public const string Register = "~/Views/Account/Register.cshtml";
            public const string AccessDenied = "~/Views/Account/AccessDenied.cshtml";
        }
        public static class Home
        {
            public const string Index = "~/Views/Home/Index.cshtml";
            public const string FAQ = "~/Views/Home/FAQ.cshtml";
            public const string Feedback = "~/Views/Home/Feedback.cshtml";
            public const string Announcements = "~/Views/Home/Announcements.cshtml";
            public const string WasteManagement = "~/Views/Home/WasteManagement.cshtml";
        }
        public static class Admin
        {
            public const string Index = "~/Views/Admin/Index.cshtml";
            public const string Users = "~/Views/Admin/Users.cshtml";
            public const string Feedback = "~/Views/Admin/Feedback.cshtml";
            public const string Announcements = "~/Views/Admin/Announcements.cshtml";
            public const string Events = "~/Views/Admin/Events.cshtml";
        }
        public static class Dashboard
        {
            public const string Index = "~/Views/Dashboard/Index.cshtml";
            public const string MyBookings = "~/Views/Dashboard/MyBookings.cshtml";
            public const string Profile = "~/Views/Dashboard/Profile.cshtml";
        }
        public static class Events
        {
            public const string Index = "~/Views/Events/Index.cshtml";
            public const string EventDetails = "~/Views/Events/EventDetails.cshtml";
            public const string Create = "~/Views/Events/Create.cshtml";
        }
        public static class Services
        {
            public const string Index = "~/Views/Services/Index.cshtml";
            public const string Book = "~/Views/Services/Book.cshtml";
            public const string ServiceDetails = "~/Views/Services/ServiceDetails.cshtml";
        }
    }
}
