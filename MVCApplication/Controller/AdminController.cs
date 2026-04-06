using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling administrative actions such as managing users, events, and feedback.
    /// </summary>
    public class AdminController : BaseAppController
    {
        /// <summary>
        /// Initializes a new instance of the AdminController class with the provided database context.
        /// </summary>
        /// <param name="db">Application database context</param>
        public AdminController(AppDb db) : base(db)
        {
        }
        /// <summary>
        /// Displays the main admin dashboard. Requires admin access.
        /// </summary>
        /// <returns>Admin dashboard view</returns>
        [HttpGet("/Admin")]
        public Task<IActionResult> Index() => GraveMind(Admin.Index, "admin", "Admin", admin: true);

        /// <summary>
        /// Displays all announcements for admin management. Requires admin access.
        /// </summary>
        /// <returns>Announcements view</returns>
        [HttpGet("/Admin/Announcements")]
        public Task<IActionResult> Announcements() => GraveMind(Admin.Announcements, "announcements", "Announcements",  admin: true);

        /// <summary>
        /// Displays all users and populates the model with user data from the database. Requires admin access.
        /// </summary>
        /// <returns>Users view with data</returns>
        [HttpGet("/Admin/Users")]
        public Task<IActionResult> Users() => GraveMind(Admin.Users, "users", "View all users", admin: true, 
            populate: async m => { var (users, _) = await _db.GetUser(null); m.Users = users ?? []; }, errorMsg: "No users Found");

        /// <summary>
        /// Handles user management actions such as create, update, and delete.
        /// </summary>
        /// <param name="id">List of user IDs for delete operations</param>
        /// <param name="users">List of users for update operations</param>
        /// <param name="user">Single user for create operations</param>
        /// <param name="action">Action to perform (create, update, delete)</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Admin/Users")]
        public Task<IActionResult> Users(List<int>? id, List<User> users, User? user, string action) => string.IsNullOrEmpty(action)
            ? GraveMind(Admin.Users, "users", "View All Users", admin: true,
                populate: async m => { var (users, _) = await _db.GetUser(null); m.Users = users ?? []; },
                errorMsg: "No action specified")
            : action == "delete" && id != null
                ? GraveMind(Admin.Users, admin: true,
                    validMsg: "Successfully deleted user",
                    save: () => _db.DeleteUsers(id),
                    redirct: () => RedirectToAction("Users"))
            : action == "update" && users != null
                ? GraveMind(Admin.Users, admin: true,
                    validMsg: "Successfully updated user",
                    save: async () => await _db.UpdateUser(users) > 0,
                    redirct: () => RedirectToAction("Users"))
            : action == "create" && user != null
                ? GraveMind(Admin.Users, admin: true,
                    validMsg: "Successfully created user",
                    save: () => _db.SaveUser(user),
                    redirct: () => RedirectToAction("Users"))
            : Task.FromResult(NotFound() as IActionResult);

        /// <summary>
        /// Displays all events and populates the model with event data from the database. Requires admin access.
        /// </summary>
        /// <returns>Events view with data</returns>
        [HttpGet("/Admin/Events")]
        public Task<IActionResult> Events() => GraveMind(Admin.Events, "events", "Events", admin: true,
            populate: async m => { var (e, _) = await _db.GetEvent(null); m.Events = e ?? []; }, errorMsg: "No events Found");

        /// <summary>
        /// Handles event management actions such as create, update, and delete.
        /// </summary>
        /// <param name="id">List of event IDs for delete operations</param>
        /// <param name="events">List of events for update operations</param>
        /// <param name="singleEvent">Single event for create operations</param>
        /// <param name="action">Action to perform (create, update, delete)</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Admin/Events")]
        public Task<IActionResult> Events(List<int>? id, List<Event> events, Event? singleEvent, string action) => string.IsNullOrEmpty(action)
            ? GraveMind(Admin.Events, "events", "View All Events", admin: true,
                populate: async m => { var (Events, _) = await _db.GetEvent(null); m.Events = Events ?? []; },
                errorMsg: "No action specified")
            : action == "delete" && id != null
                ? GraveMind(Admin.Events, admin: true,
                    validMsg: "Successfully deleted event",
                    save: () => _db.DeleteEvents(id),
                    redirct: () => RedirectToAction("Events"))
            : action == "update" && events != null
                ? GraveMind(Admin.Events, admin: true,
                    validMsg: "Successfully updated event",
                    save: async () => await _db.UpdateEvent(events) > 0,
                    redirct: () => RedirectToAction("Events"))
            : action == "create" && singleEvent != null
                ? GraveMind(Admin.Events, admin: true,
                    validMsg: "Successfully created event",
                    save: () => _db.SaveEvent(singleEvent),
                    redirct: () => RedirectToAction("Events"))
            : Task.FromResult(NotFound() as IActionResult);

        /// <summary>
        /// Displays all feedback entries and populates the model with feedback data from the database. Requires admin access.
        /// </summary>
        /// <returns>Feedback view with data</returns>
        [HttpGet("/Admin/FeedBack")]
        public Task<IActionResult> Feedback() => GraveMind(Admin.Feedback, "feedbacks", "Feedback Forms", admin: true,
            populate: async m => { var (f, _) = await _db.GetFeedback(null); m.Feedbacks = f ?? []; }, errorMsg: "No feedbacks Found");

        /// <summary>
        /// Handles feedback management actions such as create, update, and delete.
        /// </summary>
        /// <param name="id">List of feedback IDs for delete operations</param>
        /// <param name="feedbacks">List of feedback entries for update operations</param>
        /// <param name="feedback">Single feedback entry for create operations</param>
        /// <param name="action">Action to perform (create, update, delete)</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Admin/FeedBack")]
        public  Task<IActionResult> Feedback(List<int>? id, List<Feedback> feedbacks, Feedback? feedback, string action) => string.IsNullOrEmpty(action)
            ? GraveMind(Admin.Events, "feedbacks", "View All Feedbacks", admin: true,
                populate: async m => { var (f, _) = await _db.GetFeedback(null); m.Feedbacks = f ?? []; },
                errorMsg: "No action specified")
            : action == "delete" && id != null
                ? GraveMind(Admin.Feedback, admin: true,
                    validMsg: "Successfully deleted feedback",
                    save: () => _db.DeleteFeedbacks(id),
                    redirct: () => RedirectToAction("Feedback"))
            : action == "update" && feedbacks != null
                ? GraveMind(Admin.Feedback, admin: true,
                    validMsg: "Successfully updated feedback",
                    save: async () => await _db.UpdateFeedback(feedbacks) > 0,
                    redirct: () => RedirectToAction("Feedback"))
            : action == "create" && feedback != null
                ? GraveMind(Admin.Feedback, admin: true,
                    validMsg: "Successfully created feedback",
                    save: () => _db.SaveFeedback(feedback),
                    redirct: () => RedirectToAction("Feedback"))
            : Task.FromResult(NotFound() as IActionResult);
    }
}