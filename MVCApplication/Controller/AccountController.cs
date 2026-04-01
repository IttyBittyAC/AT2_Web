using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    public class AccountController : BaseAppController
    {
        public AccountController(AppDb db) : base(db)
        {
        }

        [HttpGet("/Account/Register")]
        public IActionResult Register(string? returnUrl = null)
        {
            if (IsAuth)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = Build(returnUrl: returnUrl);
            return View(Account.Register, model);
        }

        [HttpPost("/Account/Register")]
        public async Task<IActionResult> Register(
            string password,
            string email,
            string username,
            string fullname,
            string? returnUrl = null,
            string? adminPassword = null)
        {
            string placeholderPass = "test";
            string role = "user";

            if (!string.IsNullOrEmpty(adminPassword))
            {
                if (adminPassword.ToLower().Trim() == placeholderPass)
                {
                    role = "admin";
                }
            }

            if (string.IsNullOrEmpty(email) ||
                string.IsNullOrEmpty(password) ||
                string.IsNullOrEmpty(fullname) ||
                string.IsNullOrEmpty(username))
            {
                var invalidModel = Build(returnUrl: returnUrl);
                invalidModel.Error = "Fields Required";
                return View(Account.Register, invalidModel);
            }

            var register = await _db.Register(password, email, username, fullname, role);

            if (register == null)
            {
                var errorModel = Build(returnUrl: returnUrl);
                errorModel.Error = "Cannot Register";
                return View(Account.Register, errorModel);
            }

            return RedirectToAction("Login");
        }

        [HttpGet("/Account/Login")]
        public IActionResult Login(string? returnUrl = null)
        {
            if (IsAuth)
            {
                return RedirectToAction("Index", "Home");
            }

            var model = Build(returnUrl: returnUrl);
            return View(Account.Login, model);
        }

        [HttpPost("/Account/Login")]
        public async Task<IActionResult> Login(string password, string email, string? returnUrl = null)
        {
            if (string.IsNullOrEmpty(email) || string.IsNullOrEmpty(password))
            {
                var invalidModel = Build(returnUrl: returnUrl);
                invalidModel.Error = "Email and password required.";
                return View(Account.Login, invalidModel);
            }

            var user = await _db.Login(password, email);

            if (user == null)
            {
                var errorModel = Build(returnUrl: returnUrl);
                errorModel.Error = "Invalid Login.";
                return View(Account.Login, errorModel);
            }

            HttpContext.Session.SetString("user", user.Email ?? "");
            HttpContext.Session.SetString("role", user.Role ?? "");

            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
            {
                return Redirect(returnUrl);
            }

            return RedirectToAction("Index", "Home");
        }

        [HttpPost("/Account/Logout")]
        public IActionResult Logout()
        {
            IActionResult? redirect = Guard();
            if (redirect != null)
            {
                return redirect;
            }

            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }
    }
}