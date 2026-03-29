using System.Diagnostics;
using System.Net.WebSockets;
using System.Reflection.Metadata.Ecma335;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using MVCApplication.Data;
using MVCApplication.Models;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// I Have a dream to build an abomination truly something to think about 
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

        [HttpGet]
        public IActionResult Index()
        {
            return View("~/Views/Home/Index.cshtml");
        }

        [HttpGet("/Register")]
        public IActionResult Register(string? returnurl = null) => IsAuth ? RedirectToAction("Index") : View("~/Views/Account/Register.cshtml", Build(returnUrl: returnurl));

        [HttpPost("/Register")]
        public async Task<IActionResult> Register(string password, string email, string username, string fullname, string? returnurl = null,string? adminPassword = null)
        {
            string placeholderpass = "test"; 
            string role = string.IsNullOrEmpty(adminPassword) ? "user" : adminPassword.ToLower().Trim() == placeholderpass ? "admin" : "user";
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password) || string.IsNullOrEmpty(fullname) || string.IsNullOrEmpty(username))
            {
                var model = Build(returnUrl: returnurl);
                model.Error = ("Fields Required");
                return View("~/Views/Account/Register.cshtml", model);
            }
            var register = await _db.Register(password, email, username, fullname, role);
            return register == null ? View("~/Views/Account/Register.cshtml", new Func<AndInTheDarknessBindThem>(() => { var m = Build(returnUrl: returnurl); m.Error = "Cannot Register"; return m; })()) : RedirectToAction("Login");
        }

        [HttpGet("/Login")]
        public IActionResult Login(string? returnurl = null) => IsAuth ? RedirectToAction("Index") : View("~/Views/Account/Login.cshtml",Build(returnUrl: returnurl));

        [HttpPost("/Login")]
        public async Task<IActionResult> Login(string password, string email, string? returnurl = null)
        {
                       
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                var model = Build(returnUrl: returnurl);
                model.Error = ( "Email and password required.");
                return View("~/Views/Account/Login.cshtml", model);
            }
            var user = await _db.Login(password, email);

            if (user != null)
            {
                var model = Build(returnUrl: returnurl);
                model.Error = ("Invalid Login.");
                return View("~/Views/Account/Login.cshtml", model);
            }
            ;
            HttpContext.Session.SetString("user", user?.Email ?? "");
            HttpContext.Session.SetString("role", user?.Role ?? "");

            return !string.IsNullOrEmpty(returnurl) && Url.IsLocalUrl(returnurl) ? Redirect(returnurl) : RedirectToAction("Index");
        }
            
        [HttpPost("/Logout")]
        public IActionResult Logout() 
        {
            HttpContext.Session.Clear();
            return RedirectToAction("~/Views/Account/Login.cshtml");
        }
        [HttpGet("/")]
        public async Task<IActionResult> GetAll()
        {
            return View();
        }

        #region unimplemented 
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Feedback feedback)
        {
            return RedirectToAction("ThankYou");
        }
        public IActionResult EventDetails(int id)
        {
            return View();
        }
        public IActionResult ThankYou()
        {
            return View();
        }
        public IActionResult ServiceDetails(int id)
        {
            return View();
        }
        public IActionResult Announcements()
        {
            return View();
        }

        public IActionResult Book()
        {
            return View();
        }
        public IActionResult Feedback()
        {
            return View();
        }
        public IActionResult FAQ()
        {
            return View();
        }
        public IActionResult Users()
        {
            return View();
        }

        public IActionResult Events()
        {
            return View();
        }
        public IActionResult Confirmation()
        {
            return View();
        }
        public IActionResult MyBookings()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Profile()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
        #endregion
    }
}
