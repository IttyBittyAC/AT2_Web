using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using MVCApplication.Data;
using MVCApplication.Models;
using System.Data.SqlTypes;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling user account-related actions such as registration, login, and logout.
    /// </summary>
    public class AccountController : BaseAppController
    {
        /// <summary>
        /// Initializes a new instance of the AccountController class with the provided database context,
        /// </summary>  
        /// <param name="db">Application database context</param>
        public AccountController(AppDb db) : base(db)
        {
        }

        /// <summary>
        /// Handles user registration by displaying the registration form. 
        /// If the user is already authenticated, they are redirected to the home page.
        /// </summary>
        /// <param name="returnurl">Optional return URL after successful registration</param>
        [HttpGet("/Account/Register")]
        public IActionResult Register(string? returnurl = null) => IsAuth ? RedirectToAction("Index", "Home") : View(Account.Register, Build("users", "Register", returnUrl: returnurl));

        /// <summary>
        /// Handles user by validating input and saving the new user to the database. 
        /// If any required fields are missing, an error message is displayed.
        /// </summary>
        /// <param name="password">User password</param>
        /// <param name="email">User email address</param>
        /// <param name="username">Username for the account</param>
        /// <param name="fullname">Full name of the user</param>
        /// <param name="returnUrl">Optional return URL after registration</param>
        /// <param name="adminPassword">Optional admin password to assign admin role</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
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

        /// <summary>
        /// Displays the login page or redirects if the user is already authenticated.
        /// </summary>
        /// <param name="returnUrl">Optional return URL after login</param>
        [HttpGet("/Account/Login")]
        public IActionResult Login(string? returnUrl = null) => IsAuth ? RedirectToAction("Index", "Home") : View(Account.Login, Build(returnUrl: returnUrl));

        /// <summary>
        /// Handles user login by validating credentials and setting session data.
        /// </summary>
        /// <param name="password">User password</param>
        /// <param name="email">User email address</param>
        /// <param name="returnurl">Optional return URL after login</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
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

        /// <summary>
        /// Logs the user out by clearing the session and redirects to the login page.
        /// </summary>
        [HttpPost("/Account/Logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}