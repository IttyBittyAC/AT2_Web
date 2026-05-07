using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using static MVCApplication.Helpers.V;
using static MVCApplication.Helpers.MessageDictionary;
using static MVCApplication.Helpers.UserRole;
using Microsoft.Extensions.Options;
using MVCApplication.Models.Seeding;
using MVCApplication.Helpers;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling user account-related actions such as registration, login, and logout.
    /// </summary>
    public class AccountController : BaseAppController<AccountController>
    {
        private readonly AdminSettings _adminSettings;
        /// <summary>
        /// Initializes a new instance of the AccountController class with the provided database context,
        /// </summary>  
        /// <param name="db">Application database context</param>
        /// <param name="adminSettings">Admin Password string</param>
        /// <param name="logger">Logging Context for Controller Logger</param>
        public AccountController(AppDb db, ILogger<AccountController> logger, IOptions<AdminSettings> adminSettings) : base(db,logger)
        {
            _adminSettings = adminSettings.Value;
        }

        /// <summary>
        /// Handles user registration by displaying the registration form. 
        /// If the user is already authenticated, they are redirected to the home page.
        /// </summary>
        /// <param name="returnUrl">Optional return URL after successful registration</param>
        [HttpGet("/Account/Register")]
        public Task<IActionResult> Register(string? returnUrl = null) => !IsAuth ? GraveMind(Account.Register, MethodCode.Register, redirect: () => Redirect(returnUrl)) : Task.FromResult<IActionResult>(RedirectToAction("Index", "Home"));

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
            ? GraveMind(Account.Register, MethodCode.RegisterBlocked,
                populate: async m => { m.Error = Store[MethodCode.RegisterBlocked].ErrorMsg; await Task.CompletedTask; })
            : GraveMind(Account.Register, MethodCode.Register, 
                save: async () => await _db.Register(password, email, username, fullname, !string.IsNullOrEmpty(adminPassword) && adminPassword.Trim() == _adminSettings.SeedPassword ? admin.ToString() : user.ToString()) != null, redirect: () => RedirectToAction("Login"));

        /// <summary>
        /// Displays the login page or redirects if the user is already authenticated.
        /// </summary>
        /// <param name="returnUrl">Optional return URL after login</param>
        [HttpGet("/Account/Login")]
        public Task<IActionResult> Login(string? returnUrl = null) => !IsAuth ? GraveMind(Account.Login, MethodCode.Login, redirect: () => Redirect(returnUrl)) : Task.FromResult<IActionResult>(RedirectToAction("Index", "Home"));

        /// <summary>
        /// Handles user login by validating credentials and setting session data.
        /// </summary>
        /// <param name="password">User password</param>
        /// <param name="email">User email address</param>
        /// <param name="returnurl">Optional return URL after login</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Account/Login")]
        public Task<IActionResult> Login(string password, string email, string? returnurl = null) =>
            string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password)
                ? GraveMind(Account.Login, MethodCode.Login,
                    populate: m => { m.Error = Store[MethodCode.LoginInvalid].ErrorMsg; return Task.CompletedTask; })
                : GraveMind(Account.Login, MethodCode.Login,
                    save: async () =>
                    {
                        var user = await _db.Login(password, email);

                        if (user == null)
                            return false;

                        HttpContext.Session.SetString(SessionKeys.Type, user.Email ?? "");
                        HttpContext.Session.SetString(SessionKeys.Role, user.Role ?? "");
                        
                        return true;
                    },
                    redirect: () =>
                        !string.IsNullOrEmpty(returnurl) && Url.IsLocalUrl(returnurl)
                            ? Redirect(returnurl)
                            : RedirectToAction("Index", "Home"));

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