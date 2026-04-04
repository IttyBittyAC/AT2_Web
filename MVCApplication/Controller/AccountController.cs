using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    public class AccountController : BaseAppController
    {
        public AccountController(AppDb db) : base(db)
        {
        }

        [HttpGet("/Account/Register")]
        public IActionResult Register(string? returnurl = null) => IsAuth ? RedirectToAction("Index", "Home") : View(Account.Register, Build("users", "Register", returnUrl: returnurl));

        [HttpPost("/Account/Register")]
        public Task<IActionResult> Register(
            string password,
            string email,
            string username,
            string fullname,
            string? returnUrl = null,
            string? adminPassword = null) =>
            string.IsNullOrEmpty(email) ||
            string.IsNullOrEmpty(password) ||
            string.IsNullOrEmpty(username) ||
            string.IsNullOrEmpty(fullname)
            ? GraveMind(Account.Register, "users", "Register",
                populate: async m => { m.Error = "All Fields are required"; await Task.CompletedTask; })
            : GraveMind(Account.Register, "users", "Register", 
                save: async () => await _db.Register(password, email, username, fullname, !string.IsNullOrEmpty(adminPassword) && adminPassword.ToLower().Trim() == "test" ? "admin" : "user") != null, validMsg: "Registered completed", redirct: () => RedirectToAction("Login"), errorMsg: "Cannot Register");
        

        [HttpGet("/Account/Login")]
        public IActionResult Login(string? returnUrl = null) => IsAuth ? RedirectToAction("Index", "Home") : View(Account.Login, Build(returnUrl: returnUrl));


        [HttpPost("/Account/Login")]
        public async Task<IActionResult> Login(string password, string email, string? returnurl = null) => string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)
            ? View(Account.Login, new Func<AndInTheDarknessBindThem>(() => { var m = Build(returnUrl: returnurl); m.Error = "Email and Password Required"; return m; })())
            : await _db.Login(password, email) is { } user
                ? new Func<IActionResult>(() => {
                    HttpContext.Session.SetString("user", user.Email ?? "");
                    HttpContext.Session.SetString("role", user.Role ?? "");
                    return !string.IsNullOrEmpty(returnurl) && Url.IsLocalUrl(returnurl) ? Redirect(returnurl) : RedirectToAction("Index", "Home");
                })() 
                : View(Account.Login, new Func<AndInTheDarknessBindThem>(() => { 
                    var m = Build(returnUrl: returnurl); 
                    m.Error = "Login Invalid";
                    return m; })()); 

        [HttpPost("/Account/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}