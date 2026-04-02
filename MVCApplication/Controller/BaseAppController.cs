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
        protected async Task<IActionResult> Ring(string view, bool? reqauth = null, string? table = null, string? title = null, string? returnurl = null, bool? requireadmin = null, AndInTheDarknessBindThem? passedparam = null, List<int>? id = null, string? action = null, string? email = null, string? username = null, string? fullname = null, string? password = null) =>
            reqauth == null ? View(view, Build(table ?? "Not set", title ?? "Not set", returnUrl: returnurl ?? null)) : requireadmin != null ? new Func<IActionResult>(() => { return (View(view)); }) () :

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