using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    public class AdminController : BaseAppController
    {
        public AdminController(AppDb db) : base(db)
        {
        }

        [HttpGet("/Admin")]
        public Task<IActionResult> Index() => GraveMind(Admin.Index, "admin", "Admin", admin: true);

        [HttpGet("/Admin/Announcements")]
        public Task<IActionResult> Announcements() => GraveMind(Admin.Announcements, "announcements", "Announcements",  admin: true);

        [HttpGet("/Admin/Users")]
        public Task<IActionResult> Users() => GraveMind(Admin.Users, "users", "View all users", admin: true, 
            populate: async m => { var (users, _) = await _db.GetUser(null); m.Users = users ?? []; }, errorMsg: "No users Found");

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

        [HttpGet("/Admin/Events")]
        public Task<IActionResult> Events() => GraveMind(Admin.Events, "events", "Events", admin: true,
            populate: async m => { var (e, _) = await _db.GetEvent(null); m.Events = e ?? []; }, errorMsg: "No events Found");

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

        [HttpGet("/Admin/FeedBack")]
        public Task<IActionResult> Feedback() => GraveMind(Admin.Feedback, "feedbacks", "FeedBack Forms", admin: true,
            populate: async m => { var (f, _) = await _db.GetFeedback(null); m.Feedbacks = f ?? []; }, errorMsg: "No feedbacks Found");

        [HttpPost("/Admin/FeedBack")]
        public  Task<IActionResult> Feedback(List<int>? id, List<Feedback> feedbacks, Feedback? feedback, string action) => string.IsNullOrEmpty(action)
            ? GraveMind(Admin.Events, "feedback", "View All Feedbacks", admin: true,
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