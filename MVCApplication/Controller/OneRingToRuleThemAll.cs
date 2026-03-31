using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Helpers;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// I Have a dream to build an abominationm, truly something to think about 
    /// </summary>
    public class OneRingToRuleThemAll : Controller
    {
        
        private readonly AppDb _db;
        private bool IsAuth => HttpContext.Session.GetString("user") != null;
        private bool IsAdmin => HttpContext.Session.GetString("role") == "admin";
        public OneRingToRuleThemAll(AppDb db) => _db = db;
        private AndInTheDarknessBindThem Build(string table = "", string? title = null, string? returnUrl = null) =>
            AndInTheDarknessBindThem.Build(
                table,
                HttpContext.Session.GetString("user"),
                HttpContext.Session.GetString("role"),
                title,
                returnUrl
            );

        #region Home
        [HttpGet("/")]
        [HttpGet("/Home")]
        public IActionResult Index()
        {
            return View(Home.Index, Build("home", "Home"));
        }
        [HttpGet("/Home/Announcements")]
        public IActionResult Announcements() => View(Home.Announcements, Build("announcements", "We have something to announce"));

        [HttpGet("/Home/FeedBack")]
        public IActionResult Feedback() => View(Home.Feedback, Build("feedback", "Feedback Form"));

        [HttpPost("/Home/FeedBack")]
        public async Task<IActionResult> Feedback(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                var m = Build("feedback", "Feedback");
                m.Error = "Please fill in all fields";
                m.Feedback = feedback;
                return View(Home.Feedback, m);
            }

            var saved = await _db.SaveFeedback(feedback);
            SetSuccess(saved, "Thank you for making a feedback");
            return RedirectToAction("Feedback");
        }
        [HttpGet("/Home/FAQ")]
        public IActionResult FAQ() => View(Home.FAQ, Build("faq", "FAQ"));

        #endregion 
        #region Admin
        [HttpGet("/Admin")]
        public IActionResult Admin() => Guard(true) is { } redirct ? redirct : View(V.Admin.Index, Build("admin", "Admin"));

        [HttpGet("/Admin/Announcements")]
        public IActionResult Announcement() => Guard(true) is { } redirct ? redirct : View(V.Admin.Announcements, Build("booking", "Annoucements"));

        [HttpGet("/Admin/Users")]
        public async Task<IActionResult> AllUsers()
        {
            if (Guard(true) is { } redirct) return redirct;

            var model = Build("users", "View all users");
            var (users, single) = await _db.GetUser(null);
            model.Users = users ?? [];
            return View(V.Admin.Users, model);
        }
        [HttpPost("/Admin/Users")]
        public async Task<IActionResult> AllUsers(List<int>? id, List<User?> _users, User? _user, string action)
        {
            if (Guard(true) is { } redirct) return redirct;

            var model = Build("users", "View all users");

            var done = action == "delete" && id != null ? await _db.DeleteUsers(id) : action == "update" && _user != null ? await _db.UpdateUser(_users) > 0 : action == "create" && _user != null ? await _db.SaveUser(_user) : false;

            var (users, single) = await _db.GetUser(null);
            model.Users = users ?? [];
            SetSuccess(done, "Completed operations");
            return View(V.Admin.Users, model);
        }
        [HttpGet("/Admin/Events")]
        public async Task<IActionResult> AllEvents()
        {
            if (Guard(true) is { } redirct) return redirct;

            var model = Build("events", "Events");
            var (events, single) = await _db.GetEvent(null);
            model.Events = events ?? [];
            return View(V.Admin.Events, model);
        }
        [HttpPost("/Admin/Events")]
        public async Task<IActionResult> AllEvents(List<int>? id, List<Event?> _events, Event? _event, string action)
        {
            if (Guard(true) is { } redirct) return redirct;

            var model = Build("events", "Events");

            var done = action == "delete" && id != null ? await _db.DeleteEvents(id) : action == "update" && _events != null ? await _db.UpdateEvent(_events) > 0 : action == "create" && _event != null ? await _db.SaveEvent(_event) : false;

            var (events, single) = await _db.GetEvent(null);
            model.Events = events ?? [];
            SetSuccess(done, "Completed operations");
            return View(V.Admin.Events, model);
        }
        [HttpGet("/Admin/FeedBack")]
        public async Task<IActionResult> AllFeedBack()
        {
            if (Guard(true) is { } redirct) return redirct;
            var model = Build("feedbacks", "FeedBack Forms");
            var (feedbacks, single) = await _db.GetFeedback(null);
            model.Feedbacks = feedbacks ?? [];
            return View(V.Admin.Feedback, model);
        }
        [HttpPost("/Admin/FeedBack")]
        public async Task<IActionResult> AllFeedBack(List<int>? id, List<Feedback?> _feedbacks, Feedback? _feedback, string action)
        {
            if (Guard(true) is { } redirct) return redirct;
            var model = Build("feedbacks", "FeedBack Forms");

            var done = action == "delete" && id != null ? await _db.DeleteFeedbacks(id) : action == "update" && _feedbacks != null ? await _db.UpdateFeedback(_feedbacks) > 0 : action == "create" && _feedback != null ? await _db.SaveFeedback(_feedback) : false;

            var (feedbacks, single) = await _db.GetFeedback(null);
            model.Feedbacks = feedbacks ?? [];
            SetSuccess(done, "Completed operations");
            return View(V.Admin.Feedback, model);
        }

        #endregion 
        #region Account
        [HttpGet("/Account/Register")]
        public IActionResult Register(string? returnurl = null) => IsAuth ? RedirectToAction("Index") : View(Account.Register, Build(returnUrl: returnurl));

        [HttpPost("/Account/Register")]
        public async Task<IActionResult> Register(string password, string email, string username, string fullname, string? returnurl = null, string? adminPassword = null)
        {
            string placeholderpass = "test";
            string role = string.IsNullOrEmpty(adminPassword) ? "user" : adminPassword.ToLower().Trim() == placeholderpass ? "admin" : "user";
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(username))
            {
                var model = Build(returnUrl: returnurl);
                model.Error = ("Fields Required");
                return View(Account.Register, model);
            }
            var register = await _db.Register(password, email, username, fullname, role);
            return register == null ? View(Account.Register, new Func<AndInTheDarknessBindThem>(() => { var m = Build(returnUrl: returnurl); m.Error = "Cannot Register"; return m; })()) : RedirectToAction("Login");
        }

        [HttpGet("/Account/Login")]
        public IActionResult Login(string? returnurl = null) => IsAuth ? RedirectToAction("Index") : View(Account.Login, Build(returnUrl: returnurl));

        [HttpPost("/Account/Login")]
        public async Task<IActionResult> Login(string password, string email, string? returnurl = null)
        {

            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                var model = Build(returnUrl: returnurl);
                model.Error = ("Email and password required.");
                return View(Account.Login, model);
            }
            var user = await _db.Login(password, email);

            if (user == null)
            {
                var model = Build(returnUrl: returnurl);
                model.Error = ("Invalid Login.");
                return View(Account.Login, model);
            }
            ;
            HttpContext.Session.SetString("user", user?.Email ?? "");
            HttpContext.Session.SetString("role", user?.Role ?? "");

            return !string.IsNullOrEmpty(returnurl) && Url.IsLocalUrl(returnurl) ? Redirect(returnurl) : RedirectToAction("Index");
        }

        [HttpPost("/Account/Logout")]
        public IActionResult Logout()
        {
            if (Guard() is { } redirct) return redirct;
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
        #endregion 
        #region Dashboard
        [HttpGet("/Dashboard")]
        public IActionResult Dashboard() => Guard() is { } redirect ? redirect : View(V.Dashboard.Index, Build("dashboard", "Dashboard"));

        [HttpGet("/DashBoard/MyBookings")]
        public IActionResult MyBookings()
        {
            if (Guard() is { } redirct) return redirct;
            return View(V.Dashboard.MyBookings, Build("bookings", "My Bookings"));
        }

        [HttpGet("/Dashboard/Profile")]
        public IActionResult Profile()
        {
            if (Guard() is { } redirct) return redirct;
            return View(V.Dashboard.Profile, Build("users", "Profile"));
        }
        #endregion
        #region Services
        [HttpGet("/Services")]
        public async Task<IActionResult> Services(int? id) 
        {
            var model = Build("bookings", "List of all bookings select one to view details");
            var (bookings, booking) = await _db.GetBooking(id);
            if (id == null) model.Bookings = bookings ?? [];
            else model.Booking = booking;
            return View(V.Services.Index, model);
        }
        [HttpPost("/Services")]
        public IActionResult Services(int id, string action) => action == "details" ? RedirectToAction("Details", new { id }) : RedirectToAction("Book");

        [HttpGet("/Services/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = Build("bookings", "Services");

            var (bookings, booking) = await _db.GetBooking(id);

            if (booking == null)
                return NotFound();

            model.Booking = booking;

            return View(V.Services.ServiceDetails, model);
        }

        [HttpGet("/Services/Book")]
        public IActionResult Book() => Guard() is { } redirct ? redirct : View(V.Services.Book, Build("bookings", "Book smt"));

        [HttpPost("/Services/Book")]
        public async Task<IActionResult> Book(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                var m = Build("bookings", "Book something");
                m.Error = "Please fill in all fields";
                m.Booking = booking;
                return View(V.Services.Book, m);
            }
            var saved = await _db.SaveBooking(booking);
            SetSuccess(saved, "Thank you for making a booking");
            return RedirectToAction("Book");
        }
        #endregion 
        #region Events
        [HttpGet("/Events")]
        public async Task<IActionResult> Events()
        {
            var model = Build("events", "Events");
            var (events, single) = await _db.GetEvent(null);
            model.Events = events ?? []; 
            return View(V.Events.Index, model);
        }
        [HttpGet("/Events/Create")]
        public IActionResult CreateEvent() => Guard() is { } redirct ? redirct : View(V.Events.Create, Build("events", "make an event"));

        [HttpPost("/Events/Create")]
        public async Task<IActionResult> CreateEvent(Event _event)
        {
            if (!ModelState.IsValid)
            {
                var m = Build("events", "Events");
                m.Error = "Please fill in all fields";
                m.Event = _event;
                return View(V.Events.Create, m);
            }

            var saved = await _db.SaveEvent(_event);
            SetSuccess(saved, "Made Event Good Luck");
            return RedirectToAction("CreateEvent");
        }

        [HttpGet("/Events/Details")]
        public IActionResult EventDisplay() => Guard() is { } redirct ? redirct : View(V.Events.EventDetails, Build("events", "event details"));

        [HttpPost("/Events/Details")]
        public async Task<IActionResult> EventDisplay(int? id)
        {
            var model = Build("events", "Display Events/Event");
            var(events, singleEvent) = await _db.GetEvent(id);
            if (id == null) model.Events = events ?? [];
            else model.Event = singleEvent;
            return View(V.Events.EventDetails, model);
        }
        #endregion


        [HttpGet("/Confirmation")]
        public IActionResult Confirmation()
        {
            return View();
        }
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true )]
        [HttpGet("/Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }

        private IActionResult? Guard(bool requireAdmin = false)
        {
            if (!IsAuth) return RedirectToAction("Login");
            if (requireAdmin && !IsAdmin) return Forbid();
            return null;  
        }
        private void SetSuccess(bool saved, string msg) => TempData["Success"] = saved ? msg : null;
    }
}
