using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;
using static MVCApplication.Helpers.MessageDictionary;
using static MVCApplication.Helpers.ActionEnums;

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
        /// <param name="logger">Logging Context for Controller Logger</param>
        public AdminController(AppDb db, ILogger<AdminController> logger) : base(db,logger)
        {
        }
        /// <summary>
        /// Displays the main admin dashboard. Requires admin access.
        /// </summary>
        /// <returns>Admin dashboard view</returns>
        [HttpGet("/Admin")]
        public Task<IActionResult> Index() => GraveMind(
            Admin.Index,
            MethodCode.AdminIndex,
            admin: true,
            populate: async m =>
            {
                var announcements = await _db.GetAnnouncements() ?? [];
                var (events, _) = await _db.GetEvent(null);
                var (users, _) = await _db.GetUser(null);
                var (feedbacks, _) = await _db.GetFeedback(null);

                m.Announcements = announcements;
                m.Events = events ?? [];
                m.Users = users ?? [];
                m.Feedbacks = feedbacks ?? [];

                ViewBag.AnnouncementsCount = m.Announcements.Count;
                ViewBag.EventsCount = m.Events.Count;
                ViewBag.UsersCount = m.Users.Count;
                ViewBag.FeedbackCount = m.Feedbacks.Count;

                ViewBag.RecentAnnouncements = m.Announcements
                    .OrderByDescending(a => a.PostedDate)
                    .Take(5)
                    .Select(a => new
                    {
                        a.Title,
                        Summary = a.Message.Length > 80 ? a.Message.Substring(0, 80) + "..." : a.Message,
                        PublishedAt = a.PostedDate
                    })
                    .ToList();

                ViewBag.RecentEvents = m.Events
                    .OrderByDescending(e => e.EventDate)
                    .Take(5)
                    .Select(e => new
                    {
                        e.Id,
                        e.Title,
                        e.Location,
                        Start = e.EventDate
                    })
                    .ToList();
            });

        /// <summary>
        /// Displays all announcements for admin management. Requires admin access.
        /// </summary>
        /// <returns>Announcements view with data</returns>
        [HttpGet("/Admin/Announcements")]
        public Task<IActionResult> Announcements() => GraveMind(
            Admin.Announcements,
            MethodCode.AdminAnnouncements,
            admin: true,
            populate: async m =>
            {
                m.Announcements = await _db.GetAnnouncements() ?? [];
            });

        /// <summary>
        /// Handles announcement management actions such as create, update, and delete.
        /// </summary>
        /// <param name="id">List of announcement IDs for delete operations</param>
        /// <param name="announcements">List of announcements for update operations</param>
        /// <param name="announcement">Single announcement for create operations</param>
        /// <param name="action">Action to perform: create, update, or delete</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Admin/Announcements")]
        public Task<IActionResult> Announcements(
            List<int>? id,
            List<Announcement>? announcements,
            Announcement? announcement,
            AdminUserAction? action) => action == null
            ? GraveMind(Admin.Announcements, MethodCode.AdminAnnouncementsInvalid,
                admin: true,
                populate: async m =>
                {
                    m.Error = Store[MethodCode.AdminAnnouncementsInvalid].ErrorMsg;
                    m.Announcements = await _db.GetAnnouncements() ?? [];
                })

            : action == AdminUserAction.Delete && id != null
                ? GraveMind(Admin.Announcements, MethodCode.AdminAnnouncementsDelete,
                    admin: true,
                    save: () => _db.DeleteAnnouncements(id),
                    redirect: () => RedirectToAction("Announcements"))

            : action == AdminUserAction.Update && announcements != null
                ? GraveMind(Admin.Announcements, MethodCode.AdminAnnouncementsUpdate,
                    admin: true,
                    save: async () => await _db.UpdateAnnouncements(announcements) > 0,
                    redirect: () => RedirectToAction("Announcements"))

            : action == AdminUserAction.Create && announcement != null
                ? GraveMind(Admin.Announcements, MethodCode.AdminAnnouncementsCreate,
                    admin: true,
                    save: async () =>
                    {
                        announcement.PostedDate = DateTime.UtcNow;
                        return await _db.SaveAnnouncement(announcement);
                    },
                    redirect: () => RedirectToAction("Announcements"))

            : Task.FromResult(NotFound() as IActionResult);

        /// <summary>
        /// Displays all users and populates the model with user data from the database. Requires admin access.
        /// </summary>
        /// <returns>Users view with data</returns>
        [HttpGet("/Admin/Users")]
        public Task<IActionResult> Users() => GraveMind(Admin.Users, MethodCode.AdminUsers, admin: true,
            populate: async m => { var (users, _) = await _db.GetUser(null); m.Users = users ?? []; });

        /// <summary>
        /// Handles user management actions such as create, update, and delete.
        /// </summary>
        /// <param name="id">List of user IDs for delete operations</param>
        /// <param name="users">List of users for update operations</param>
        /// <param name="user">Single user for create operations</param>
        /// <param name="action">Action to perform (create, update, delete)</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Admin/Users")]
        public Task<IActionResult> Users(List<int>? id, List<User>? users, User? user, AdminUserAction? action) => action == null
            ? GraveMind(Admin.Users, MethodCode.AdminUsersInvalid, admin: true,
                populate: async m => { var (users, _) = await _db.GetUser(null); m.Users = users ?? []; })
            : action == AdminUserAction.Delete && id != null
                ? GraveMind(Admin.Users, MethodCode.AdminUsersDelete,
                    admin: true,
                    save: () => _db.DeleteUsers(id),
                    redirect: () => RedirectToAction("Users"))
            : action == AdminUserAction.Update && users != null
                ? GraveMind(Admin.Users, MethodCode.AdminUsersUpdate,
                    admin: true,
                    save: async () => await _db.UpdateUser(users) > 0,
                    redirect: () => RedirectToAction("Users"))
            : action == AdminUserAction.Create && user != null
                ? GraveMind(Admin.Users, MethodCode.AdminUsersCreate,
                    admin: true,
                    redirect: () => RedirectToAction("Users"))
            : Task.FromResult(NotFound() as IActionResult);

        /// <summary>
        /// Displays all events and populates the model with event data from the database. Requires admin access.
        /// </summary>
        /// <returns>Events view with data</returns>
        [HttpGet("/Admin/Events")]
        public Task<IActionResult> Events() => GraveMind(Admin.Events, MethodCode.AdminEvents, admin: true,
            populate: async m => { var (e, _) = await _db.GetEvent(null); m.Events = e ?? []; });

        /// <summary>
        /// Handles event management actions such as create, update, and delete.
        /// </summary>
        /// <param name="id">List of event IDs for delete operations</param>
        /// <param name="events">List of events for update operations</param>
        /// <param name="singleEvent">Single event for create operations</param>
        /// <param name="action">Action to perform (create, update, delete)</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Admin/Events")]
        public Task<IActionResult> Events(List<int>? id, List<Event>? events, Event? singleEvent, AdminUserAction? action) => action == null
            ? GraveMind(Admin.Events, MethodCode.AdminEventsInvalid, admin: true,
                populate: async m => { var (Events, _) = await _db.GetEvent(null); m.Events = Events ?? []; })
            : action == AdminUserAction.Delete && id != null
                ? GraveMind(Admin.Events,
                    MethodCode.AdminEventsDelete,
                    admin: true,
                    save: () => _db.DeleteEvents(id),
                    redirect: () => RedirectToAction("Events"))
            : action == AdminUserAction.Update && events != null
                ? GraveMind(Admin.Events,
                    MethodCode.AdminEventsUpdate,
                    admin: true,
                    save: async () => await _db.UpdateEvent(events) > 0,
                    redirect: () => RedirectToAction("Events"))
            : action == AdminUserAction.Create && singleEvent != null
                ? GraveMind(Admin.Events,
                    MethodCode.AdminEventsCreate,
                    admin: true,
                    save: async () => { (bool l, _) = await _db.SaveEvent(singleEvent);  return l; },
                    redirect: () => RedirectToAction("Events"))
            : Task.FromResult(NotFound() as IActionResult);

        /// <summary>
        /// Displays all feedback entries and populates the model with feedback data from the database. Requires admin access.
        /// </summary>
        /// <returns>Feedback view with data</returns>
        [HttpGet("/Admin/FeedBack")]
        public Task<IActionResult> Feedback() => GraveMind(Admin.Feedback, MethodCode.AdminFeedback, admin: true,
            populate: async m => { var (f, _) = await _db.GetFeedback(null); m.Feedbacks = f ?? []; });

        /// <summary>
        /// Handles feedback management actions such as create, update, and delete.
        /// </summary>
        /// <param name="id">List of feedback IDs for delete operations</param>
        /// <param name="feedbacks">List of feedback entries for update operations</param>
        /// <param name="feedback">Single feedback entry for create operations</param>
        /// <param name="action">Action to perform (create, update, delete)</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Admin/FeedBack")]
        public Task<IActionResult> Feedback(List<int>? id, List<Feedback>? feedbacks, Feedback? feedback, AdminUserAction? action) => action == null
            ? GraveMind(Admin.Feedback, MethodCode.AdminFeedbackInvalid, admin: true,
                populate: async m => { var (f, _) = await _db.GetFeedback(null); m.Feedbacks = f ?? []; })
            : action == AdminUserAction.Delete && id != null
                ? GraveMind(Admin.Feedback,
                    MethodCode.AdminFeedbackDelete,
                    admin: true,
                    save: () => _db.DeleteFeedbacks(id),
                    redirect: () => RedirectToAction("Feedback"))
            : action == AdminUserAction.Update && feedbacks != null
                ? GraveMind(Admin.Feedback,
                    MethodCode.AdminFeedbackUpdate,
                    admin: true,
                    save: async () => await _db.UpdateFeedback(feedbacks) > 0,
                    redirect: () => RedirectToAction("Feedback"))
            : action == AdminUserAction.Create && feedback != null
                ? GraveMind(Admin.Feedback,
                    MethodCode.AdminFeedbackCreate,
                    admin: true,
                    save: () => _db.SaveFeedback(feedback),
                    redirect: () => RedirectToAction("Feedback"))
            : Task.FromResult(NotFound() as IActionResult);

        /// <summary>
        /// Displays all booking entries and populates the model with booking data from the database. Requires admin access.
        /// </summary>
        /// <returns>Booking view with data</returns>
        [HttpGet("/Admin/Bookings")]
        public Task<IActionResult> Bookings() => GraveMind(Admin.Bookings, MethodCode.AdminBookings, admin: true,
            populate: async m => { var (f, _) = await _db.GetBooking(null); m.Bookings = f ?? []; });

        /// <summary>
        /// Handles booking management actions such as create, update, and delete.
        /// </summary>
        /// <param name="id">List of booking IDs for delete operations</param>
        /// <param name="bookings">List of booking entries for update operations</param>
        /// <param name="booking">Single booking entry for create operations</param>
        /// <param name="action">Action to perform (create, update, delete)</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Admin/Bookings")]
        public Task<IActionResult> Bookings(List<int>? id, List<Booking>? bookings, Booking? booking, AdminUserAction? action) => action == null
            ? GraveMind(Admin.Bookings, MethodCode.AdminBookingsInvalid, admin: true,
                populate: async m => { var (f, _) = await _db.GetBooking(null); m.Bookings = f ?? []; })
            : action == AdminUserAction.Delete && id != null
                ? GraveMind(Admin.Bookings,
                    MethodCode.AdminBookingsDelete,
                    admin: true,
                    save: () => _db.DeleteBookings(id),
                    redirect: () => RedirectToAction("Bookings"))
            : action == AdminUserAction.Update && bookings != null
                ? GraveMind(Admin.Bookings,
                    MethodCode.AdminBookingsUpdate,
                    admin: true,
                    save: async () => await _db.UpdateBooking(bookings) > 0,
                    redirect: () => RedirectToAction("Bookings"))
            : action == AdminUserAction.Create && booking != null
                ? GraveMind(Admin.Bookings,
                    MethodCode.AdminBookingsCreate,
                    admin: true,
                    save: () => _db.SaveBooking(booking),
                    redirect: () => RedirectToAction("Bookings"))
            : Task.FromResult(NotFound() as IActionResult);


        /// <summary>
        /// Displays the application logs for administrators.
        /// </summary>
        /// <returns>Application logs view</returns>
        [HttpGet("/Admin/Logs")]
        public Task<IActionResult> Logs() => GraveMind(
            Admin.Logs,
            MethodCode.AdminLogs,
            admin: true,
            populate: async m =>
            {
                m.Logs = await _db.GetLogs() ?? [];
            });
    }
}