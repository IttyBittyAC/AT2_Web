using System.Threading.Tasks.Dataflow;

namespace MVCApplication.Helpers
{

    public static class MessageDictionary
    {
        public enum MethodCode
        {
            // Home
            HomeIndex,
            HomeAnnouncements,
            HomeFeedBack,
            HomeFeedBackInvalid,
            HomeFAQ,

            // Account
            Login,
            LoginInvalid,
            Register,
            RegisterBlocked,

            // Admin
            AdminIndex,
            AdminAnnouncements,
            AdminUsers,
            AdminUsersInvalid,
            AdminUsersDelete,
            AdminUsersUpdate,
            AdminUsersCreate,
            AdminEvents,
            AdminEventsInvalid,
            AdminEventsDelete,
            AdminEventsUpdate,
            AdminEventsCreate,
            AdminFeedback,
            AdminFeedbackInvalid,
            AdminFeedbackDelete,
            AdminFeedbackUpdate,
            AdminFeedbackCreate,
            AdminLogs,

            // Services
            ServiceIndex,
            ServiceDetail,
            ServiceBook,
            ServiceBookInvalid,

            // DashBoard
            DashBoardIndex,
            DashBoardBooking,
            DashBoardProfile,
            DashBoardProfileUpdate,

            // Events
            EventsIndex,
            EventsCreate,
            EventsCreateInvalid,
            EventsDetails,
            EventsRegister,

        }
        public record Info(
            string Table,
            string Title,
            string? ErrorMsg, 
            string? SuccessMsg
            );
        public static readonly Dictionary<MethodCode,Info> Store = new()
        {
            // Home Controller
            [MethodCode.HomeIndex] = new("home", "Home", null, null),
            [MethodCode.HomeAnnouncements] = new("announcements", "We have something to announce", null, null),
            [MethodCode.HomeFeedBack] = new("feedback", "Feedback Form", "Failed to Save to DataBase", "Submitted form to our database"),
            [MethodCode.HomeFeedBackInvalid] = new("feedback", "Feedback Form", "All Fields are required", null),
            [MethodCode.HomeFAQ] = new("faq", "FAQ", null, null),

            // Account Controller
            [MethodCode.RegisterBlocked] = new("users", "Register", "All Fields are required", null),
            [MethodCode.Register] = new("users", "Register", "Cannot Register", "Registered completed"),
            [MethodCode.Login] = new("users", "Login", "Login Invalid", "Login Completed"),
            [MethodCode.LoginInvalid] = new("users", "Login", "Email And Password Required", null),

            // Admin Controller
            [MethodCode.AdminIndex] = new("admin", "Admin", null, null),
            [MethodCode.AdminAnnouncements] = new("announcements", "Announcements", null, null),
            [MethodCode.AdminUsers] = new("users", "View all users", "No users Found", "Displayed Users"),
            [MethodCode.AdminUsersInvalid] = new("users", "View All Users", "No action specified", null),
            [MethodCode.AdminUsersDelete] = new("users", "Delete a User","Could not delete", "Successfully deleted user"),
            [MethodCode.AdminUsersUpdate] = new("users", "Update a User", "Could not update", "Successfully updated user"),
            [MethodCode.AdminUsersCreate] = new("users", "Create a User", "Could not create", "Successfully created user"),
            [MethodCode.AdminEvents] = new("events", "View all Events", "No events Found", "Displayed Events"),
            [MethodCode.AdminEventsInvalid] = new("events", "View all Events", "No action specified", null),
            [MethodCode.AdminEventsDelete] = new("events", "Delete a Event", "Could not delete", "Successfully deleted Event"),
            [MethodCode.AdminEventsUpdate] = new("events", "Update a Event", "Could not update", "Successfully updated Event"),
            [MethodCode.AdminEventsCreate] = new("events", "Create a Event", "Could not create", "Successfully created Event"),
            [MethodCode.AdminFeedback] = new("feedbacks", "Feedback Forms", "No feedbacks Found", "Displayed Feedback"),
            [MethodCode.AdminFeedbackInvalid] = new("feedbacks", "View All Feedback Forms", "No action specified", null),
            [MethodCode.AdminFeedbackDelete] = new("feedbacks", "Delete a Feedback Form", "Could not delete", "Successfully deleted Feedback Form"),
            [MethodCode.AdminFeedbackUpdate] = new("feedbacks", "Update a Feedback Form", "Could not update", "Successfully updated Feedback Form"),
            [MethodCode.AdminFeedbackCreate] = new("feedbacks", "Create a Feedback Form", "Could not create", "Successfully created Feedback Form"),
            [MethodCode.AdminLogs] = new("logs", "Application Logs", "Failed to Load or retrieve Logs", "Loaded Logs"),

            // Services Controller
            [MethodCode.ServiceIndex] = new("bookings", "List of all bookings select one to view details", "Failed to Load Bookings", null),
            [MethodCode.ServiceDetail] = new("bookings", "Services", "Nothing in bookings set", null),
            [MethodCode.ServiceBook] = new("bookings", "Book Something Please", "Failed to Book", "Thank you for making a booking"),
            [MethodCode.ServiceBookInvalid] = new("bookings", "Book Something Please", "Please fill in all fields", null),

            // DashBoard Controller
            [MethodCode.DashBoardIndex] = new("dashboard", "Dashboard", null, null),
            [MethodCode.DashBoardBooking] = new("bookings", "My Bookings", "No Bookings found of user", "Retrieved All Booking from your user profile"),
            [MethodCode.DashBoardProfile] = new("users","Profile", "User not found", "Welcome to Your Profile"),
            [MethodCode.DashBoardProfileUpdate] = new("users", "Update Profile", "Could not update profile", "Profile updated successfully"),

            // Events Controller
            [MethodCode.EventsIndex] = new("events", "Events", "Error Getting all events", "Retrieved and Displayed all events"),
            [MethodCode.EventsCreate] = new("events", "Make an event", "Unable to Make Event", "Made event successfully"),
            [MethodCode.EventsCreateInvalid] = new("events", "Make an event", "Please fill in all fields", null),
            [MethodCode.EventsDetails] = new("events", "Event Details", "Event not found", "Found Event"),
            [MethodCode.EventsRegister] = new("bookings", "Register for Event", "Could not register for event", "Registered for event successfully"),
        };
    }
}
