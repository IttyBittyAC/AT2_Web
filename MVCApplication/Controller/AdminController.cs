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
        public IActionResult Index()
        {
            IActionResult? redirect = Guard(true);
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("admin", "Admin");
            return View(Admin.Index, model);
        }

        [HttpGet("/Admin/Announcements")]
        public IActionResult Announcements()
        {
            IActionResult? redirect = Guard(true);
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("announcements", "Announcements");
            return View(Admin.Announcements, model);
        }

        [HttpGet("/Admin/Users")]
        public async Task<IActionResult> Users()
        {
            IActionResult? redirect = Guard(true);
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("users", "View all users");
            var result = await _db.GetUser(null);
            model.Users = result.Item1 ?? new List<User>();

            return View(Admin.Users, model);
        }

        [HttpPost("/Admin/Users")]
        public async Task<IActionResult> Users(List<int>? id, List<User> users, User? user, string action)
        {
            IActionResult? redirect = Guard(true);
            if (redirect != null)
            {
                return redirect;
            }

            bool done = false;

            if (action == "delete" && id != null)
            {
                done = await _db.DeleteUsers(id);
            }
            else if (action == "update" && users != null)
            {
                done = await _db.UpdateUser(users) > 0;
            }
            else if (action == "create" && user != null)
            {
                done = await _db.SaveUser(user);
            }

            var model = Build("users", "View all users");
            var result = await _db.GetUser(null);
            model.Users = result.Item1 ?? new List<User>();

            SetSuccess(done, "Completed operations");
            return View(Admin.Users, model);
        }

        [HttpGet("/Admin/Events")]
        public async Task<IActionResult> Events()
        {
            IActionResult? redirect = Guard(true);
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("events", "Events");
            var result = await _db.GetEvent(null);
            model.Events = result.Item1 ?? new List<Event>();

            return View(Admin.Events, model);
        }

        [HttpPost("/Admin/Events")]
        public async Task<IActionResult> Events(List<int>? id, List<Event> events, Event? singleEvent, string action)
        {
            IActionResult? redirect = Guard(true);
            if (redirect != null)
            {
                return redirect;
            }

            bool done = false;

            if (action == "delete" && id != null)
            {
                done = await _db.DeleteEvents(id);
            }
            else if (action == "update" && events != null)
            {
                done = await _db.UpdateEvent(events) > 0;
            }
            else if (action == "create" && singleEvent != null)
            {
                done = await _db.SaveEvent(singleEvent);
            }

            var model = Build("events", "Events");
            var result = await _db.GetEvent(null);
            model.Events = result.Item1 ?? new List<Event>();

            SetSuccess(done, "Completed operations");
            return View(Admin.Events, model);
        }

        [HttpGet("/Admin/FeedBack")]
        public async Task<IActionResult> Feedback()
        {
            IActionResult? redirect = Guard(true);
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("feedbacks", "FeedBack Forms");
            var result = await _db.GetFeedback(null);
            model.Feedbacks = result.Item1 ?? new List<Feedback>();

            return View(Admin.Feedback, model);
        }

        [HttpPost("/Admin/FeedBack")]
        public async Task<IActionResult> Feedback(List<int>? id, List<Feedback> feedbacks, Feedback? feedback, string action)
        {
            IActionResult? redirect = Guard(true);
            if (redirect != null)
            {
                return redirect;
            }

            bool done = false;

            if (action == "delete" && id != null)
            {
                done = await _db.DeleteFeedbacks(id);
            }
            else if (action == "update" && feedbacks != null)
            {
                done = await _db.UpdateFeedback(feedbacks) > 0;
            }
            else if (action == "create" && feedback != null)
            {
                done = await _db.SaveFeedback(feedback);
            }

            var model = Build("feedbacks", "FeedBack Forms");
            var result = await _db.GetFeedback(null);
            model.Feedbacks = result.Item1 ?? new List<Feedback>();

            SetSuccess(done, "Completed operations");
            return View(Admin.Feedback, model);
        }
    }
}