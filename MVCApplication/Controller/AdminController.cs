using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;
using static MVCApplication.Helpers.MessageDictionary;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling administrative actions such as managing users, events, and feedback.
    /// </summary>
    public class AdminController : BaseAppController<AdminController>
    {
        /// <summary>
        /// Initializes a new instance of the AdminController class with the provided database context.
        /// </summary>
        /// <param name="db">Application database context</param>
        public AdminController(AppDb db, ILogger<AdminController> logger) : base(db,logger)
        {
        }
        /// <summary>
        /// Displays the main admin dashboard. Requires admin access.
        /// </summary>
        /// <returns>Admin dashboard view</returns>
        [HttpGet("/Admin")]
        public Task<IActionResult> Index() => GraveMind(Admin.Index, Store[MethodCode.AdminIndex].Table, Store[MethodCode.AdminIndex].Title, admin: true);

        /// <summary>
        /// Displays all announcements for admin management. Requires admin access.
        /// </summary>
        /// <returns>Announcements view</returns>
        [HttpGet("/Admin/Announcements")]
        public Task<IActionResult> Announcements() => GraveMind(Admin.Announcements, Store[MethodCode.AdminAnnouncements].Table, Store[MethodCode.AdminAnnouncements].Title, admin: true);

        /// <summary>
        /// Displays all users and populates the model with user data from the database. Requires admin access.
        /// </summary>
        /// <returns>Users view with data</returns>
        [HttpGet("/Admin/Users")]
        public Task<IActionResult> Users() => GraveMind(Admin.Users, Store[MethodCode.AdminUsers].Table, Store[MethodCode.AdminUsers].Title, admin: true,
            populate: async m => { var (users, _) = await _db.GetUser(null); m.Users = users ?? []; }, errorMsg: Store[MethodCode.AdminUsers].ErrorMsg, successMsg: Store[MethodCode.AdminUsers].SuccessMsg);

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
            ? GraveMind(Admin.Users, Store[MethodCode.AdminUsersInvalid].Table, Store[MethodCode.AdminUsersInvalid].Title, admin: true,
                populate: async m => { var (users, _) = await _db.GetUser(null); m.Users = users ?? []; },
                errorMsg: Store[MethodCode.AdminUsersInvalid].ErrorMsg)
            : action == "delete" && id != null
                ? GraveMind(Admin.Users,
                    Store[MethodCode.AdminUsersDelete].Table,
                    Store[MethodCode.AdminUsersDelete].Title,
                    admin: true,
                    errorMsg: Store[MethodCode.AdminUsersDelete].ErrorMsg,
                    successMsg: Store[MethodCode.AdminUsersDelete].SuccessMsg,
                    save: () => _db.DeleteUsers(id),
                    redirect: () => RedirectToAction("Users"))
            : action == "update" && users != null
                ? GraveMind(Admin.Users,
                    Store[MethodCode.AdminUsersUpdate].Table,
                    Store[MethodCode.AdminUsersUpdate].Title,
                    admin: true,
                    errorMsg: Store[MethodCode.AdminUsersUpdate].ErrorMsg, 
                    successMsg: Store[MethodCode.AdminUsersUpdate].SuccessMsg,
                    save: async () => await _db.UpdateUser(users) > 0,
                    redirect: () => RedirectToAction("Users"))
            : action == "create" && user != null
                ? GraveMind(Admin.Users,
                    Store[MethodCode.AdminUsersCreate].Table,
                    Store[MethodCode.AdminUsersCreate].Title,
                    admin: true,
                    errorMsg: Store[MethodCode.AdminUsersCreate].ErrorMsg,
                    successMsg: Store[MethodCode.AdminUsersCreate].SuccessMsg,
                    save: () => _db.SaveUser(user),
                    redirect: () => RedirectToAction("Users"))
            : Task.FromResult(NotFound() as IActionResult);

        /// <summary>
        /// Displays all events and populates the model with event data from the database. Requires admin access.
        /// </summary>
        /// <returns>Events view with data</returns>
        [HttpGet("/Admin/Events")]
        public Task<IActionResult> Events() => GraveMind(Admin.Events, Store[MethodCode.AdminEvents].Table, Store[MethodCode.AdminEvents].Title, admin: true,
            populate: async m => { var (e, _) = await _db.GetEvent(null); m.Events = e ?? []; }, errorMsg: Store[MethodCode.AdminEvents].ErrorMsg, successMsg: Store[MethodCode.AdminEvents].SuccessMsg);

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
            ? GraveMind(Admin.Events, Store[MethodCode.AdminEventsInvalid].Table, Store[MethodCode.AdminEventsInvalid].Title, admin: true,
                populate: async m => { var (Events, _) = await _db.GetEvent(null); m.Events = Events ?? []; },
                errorMsg: Store[MethodCode.AdminEventsInvalid].ErrorMsg)
            : action == "delete" && id != null
                ? GraveMind(Admin.Events,
                    Store[MethodCode.AdminEventsDelete].Table,
                    Store[MethodCode.AdminEventsDelete].Title,
                    admin: true,
                    errorMsg: Store[MethodCode.AdminEventsDelete].ErrorMsg,
                    successMsg: Store[MethodCode.AdminEventsDelete].SuccessMsg,
                    save: () => _db.DeleteEvents(id),
                    redirect: () => RedirectToAction("Events"))
            : action == "update" && events != null
                ? GraveMind(Admin.Events,
                    Store[MethodCode.AdminEventsUpdate].Table,
                    Store[MethodCode.AdminEventsUpdate].Title,
                    admin: true,
                    errorMsg: Store[MethodCode.AdminEventsUpdate].ErrorMsg,
                    successMsg: Store[MethodCode.AdminEventsUpdate].SuccessMsg,
                    save: async () => await _db.UpdateEvent(events) > 0,
                    redirect: () => RedirectToAction("Events"))
            : action == "create" && singleEvent != null
                ? GraveMind(Admin.Events,
                    Store[MethodCode.AdminEventsCreate].Table,
                    Store[MethodCode.AdminEventsCreate].Title,
                    admin: true,
                    errorMsg: Store[MethodCode.AdminEventsCreate].ErrorMsg, 
                    successMsg: Store[MethodCode.AdminEventsCreate].SuccessMsg,
                    save: () => _db.SaveEvent(singleEvent),
                    redirect: () => RedirectToAction("Events"))
            : Task.FromResult(NotFound() as IActionResult);

        /// <summary>
        /// Displays all feedback entries and populates the model with feedback data from the database. Requires admin access.
        /// </summary>
        /// <returns>Feedback view with data</returns>
        [HttpGet("/Admin/FeedBack")]
        public Task<IActionResult> Feedback() => GraveMind(Admin.Feedback, Store[MethodCode.AdminFeedback].Table, Store[MethodCode.AdminFeedback].Title, admin: true,
            populate: async m => { var (f, _) = await _db.GetFeedback(null); m.Feedbacks = f ?? []; }, errorMsg: Store[MethodCode.AdminFeedback].ErrorMsg, successMsg: Store[MethodCode.AdminFeedback].SuccessMsg);

        /// <summary>
        /// Handles feedback management actions such as create, update, and delete.
        /// </summary>
        /// <param name="id">List of feedback IDs for delete operations</param>
        /// <param name="feedbacks">List of feedback entries for update operations</param>
        /// <param name="feedback">Single feedback entry for create operations</param>
        /// <param name="action">Action to perform (create, update, delete)</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Admin/FeedBack")]
        public Task<IActionResult> Feedback(List<int>? id, List<Feedback> feedbacks, Feedback? feedback, string action) => string.IsNullOrEmpty(action)
            ? GraveMind(Admin.Feedback, Store[MethodCode.AdminFeedbackInvalid].Table, Store[MethodCode.AdminFeedbackInvalid].Title, admin: true,
                populate: async m => { var (f, _) = await _db.GetFeedback(null); m.Feedbacks = f ?? []; },
                errorMsg: Store[MethodCode.AdminFeedbackInvalid].ErrorMsg)
            : action == "delete" && id != null
                ? GraveMind(Admin.Feedback,
                    Store[MethodCode.AdminFeedbackDelete].Table,
                    Store[MethodCode.AdminFeedbackDelete].Title,
                    admin: true,
                    errorMsg: Store[MethodCode.AdminFeedbackDelete].ErrorMsg,
                    successMsg: Store[MethodCode.AdminFeedbackDelete].SuccessMsg,
                    save: () => _db.DeleteFeedbacks(id),
                    redirect: () => RedirectToAction("Feedback"))
            : action == "update" && feedbacks != null
                ? GraveMind(Admin.Feedback,
                    Store[MethodCode.AdminFeedbackUpdate].Table,
                    Store[MethodCode.AdminFeedbackUpdate].Title,
                    admin: true,
                    errorMsg: Store[MethodCode.AdminFeedbackUpdate].ErrorMsg,
                    successMsg: Store[MethodCode.AdminFeedbackUpdate].SuccessMsg,
                    save: async () => await _db.UpdateFeedback(feedbacks) > 0,
                    redirect: () => RedirectToAction("Feedback"))
            : action == "create" && feedback != null
                ? GraveMind(Admin.Feedback,
                    Store[MethodCode.AdminFeedbackCreate].Table,
                    Store[MethodCode.AdminFeedbackCreate].Title,
                    admin: true,
                    errorMsg: Store[MethodCode.AdminFeedbackCreate].ErrorMsg,
                    successMsg: Store[MethodCode.AdminFeedbackCreate].SuccessMsg,
                    save: () => _db.SaveFeedback(feedback),
                    redirect: () => RedirectToAction("Feedback"))
            : Task.FromResult(NotFound() as IActionResult);



        /// <summary>
        /// Displays the application logs for administrators.
        /// </summary>
        /// <returns>Application logs view</returns>
        [HttpGet("/Admin/Logs")]
        public Task<IActionResult> Logs() => GraveMind(
            Admin.Logs,
            Store[MethodCode.AdminLogs].Table,
            Store[MethodCode.AdminLogs].Title,
            errorMsg: Store[MethodCode.AdminLogs].ErrorMsg,
            successMsg: Store[MethodCode.AdminLogs].SuccessMsg,
            admin: true,
            populate: async m =>
            {
                m.Logs = await _db.GetLogs() ?? [];
            });
    }
}