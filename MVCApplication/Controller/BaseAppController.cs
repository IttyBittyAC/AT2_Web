using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Helpers;
using MVCApplication.Models;

namespace MVCApplication.Controllers
{
    public abstract class BaseAppController : Controller
    {
        protected readonly AppDb _db;

        protected BaseAppController(AppDb db)
        {
            _db = db;
        }

        protected bool IsAuth
        {
            get
            {
                return HttpContext.Session.GetString("user") != null;
            }
        }

        protected bool IsAdmin
        {
            get
            {
                return HttpContext.Session.GetString("role") == "admin";
            }
        }

        protected AndInTheDarknessBindThem Build(string table = "", string? title = null, string? returnUrl = null)
        {
            return AndInTheDarknessBindThem.Build(
                table,
                HttpContext.Session.GetString("user"),
                HttpContext.Session.GetString("role"),
                title,
                returnUrl
            );
        }

        protected IActionResult? Guard(bool requireAdmin = false)
        {
            if (!IsAuth)
            {
                return RedirectToAction("Login", "Account");
            }

            if (requireAdmin && !IsAdmin)
            {
                return Forbid();
            }

            return null;
        }

        protected void SetSuccess(bool saved, string message)
        {
            if (saved)
            {
                TempData["Success"] = message;
            }
            else
            {
                TempData["Success"] = null;
            }
        }
    }
}