using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Helpers;
using MVCApplication.Models;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Base controller that provides shared database access and common functionality for all other controllers in the application.
    /// </summary>
    public abstract class BaseAppController : Controller
    {
        /// <summary>
        /// Database context for accessing application data, shared across all controllers that inherit from BaseAppController.
        /// </summary>
        protected readonly AppDb _db;
        /// <summary>
        /// Initializes a new instance of the BaseAppController class with the provided database context, 
        /// allowing derived controllers to access the database through the _db field.
        /// </summary>
        /// <param name="db"></param>
        protected BaseAppController(AppDb db)
        {
            _db = db;
        }

        /// <summary>
        /// Gets authentication status of session and sets true if not null
        /// </summary>
        protected bool IsAuth 
        {
            get
            {
                return HttpContext.Session.GetString("user") != null;
            }
        }

        /// <summary>
        /// Gets role of session and sets true if admin
        /// </summary>
        protected bool IsAdmin
        {
            get
            {
                return HttpContext.Session.GetString("role") == "admin";
            }
        }


        /// <summary>
        /// Method to make instance of model and set essential data to properties
        /// </summary>
        /// <param name="table"> Table that is related to operations </param>
        /// <param name="title"> Title for View </param>
        /// <param name="returnUrl"> Previous url </param>
        /// <returns> AndInTheDarknessBindThem initialised model with data in params </returns>
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

        /// <summary>
        /// Method that handles
        ///     Authorisation (auth/admin)
        ///     GET model pop and validation
        ///     POST save and redirection
        ///     
        /// We Trade One Villain for Another 
        /// </summary>
        /// <param name="view"> Page to be displayed (Required) </param>
        /// <param name="table"> Table name set onto model for View </param>
        /// <param name="title"> Title of page set onto model for Views </param>
        /// <param name="auth"> If True, required authenticated user </param>
        /// <param name="admin"> If True, required authenticated admin </param>
        /// <param name="populate"> Optional delegate that populates the view model with data from db before rendering the view </param>
        /// <param name="check"> Optional delegate that validates model and returns false to trigger 404 </param>
        /// <param name="save"> Optional delegate that does POST operations </param>
        /// <param name="redirct"> Optional delegate that stores where to be redirected to after a POST </param>
        /// <param name="validMsg"> Optional message for successfull POST </param>
        /// <param name="errorMsg"> Optional message for a failed operation or validation </param>
        /// <returns> An IActionResult representing either a view, redirect, or error response </returns>

        protected async Task<IActionResult> GraveMind(string view, string table = "", string? title = null,
            bool auth = false, bool admin = false,
            Func<AndInTheDarknessBindThem, Task>? populate = null,
            Func<AndInTheDarknessBindThem, bool>? check = null,
            Func<Task<bool>>? save = null,
            Func<IActionResult>? redirct = null,
            string? validMsg = null, string? errorMsg = null)
            =>
            // Gate 1 (Auth and Admin)
            Guard(requireAdmin: admin, requireAuth: auth) is { } r 
            ? await new Func<Task<IActionResult>>(async () =>
            {
                await SaveLog(true, view, "Access denied or login required");
                return r;
            })() : save != null

            // Gate 2 POST
            ? await new Func<Task<IActionResult>>(async () =>
            {
                var m = Build(table, title);
                var s = await save();

                if (!s)
                {
                    m.Error = errorMsg;

                    await SaveLog(true, view, errorMsg ?? "Save failed");

                    return View(view, m);
                }

                SetSuccess(s, validMsg ?? "done");

                await SaveLog(false, view, validMsg ?? "done");

                return redirct == null ? RedirectToAction("Index") : redirct();
            })()

            // Gate 3 GET
            : await new Func<Task<IActionResult>>(async () =>
            {
                var m = Build(table, title);

                if (populate != null)
                {
                    await populate(m);
                }

                if (check != null && !check(m))
                {
                    m.Error = errorMsg ?? "not found";

                    await SaveLog(true, view, m.Error);

                    return NotFound();
                }

                await SaveLog(errorMsg != null, view, errorMsg ?? "Viewed page: " + (title ?? view));

                return View(view, m);
            })();
        //protected async Task<IActionResult> GraveMind(string view, string table = "", string? title = null, 
        //    bool auth = false, bool admin = false, 
        //    Func<AndInTheDarknessBindThem, Task>? populate = null, 
        //    Func<AndInTheDarknessBindThem, bool>? check = null, 
        //    Func<Task<bool>>? save = null, 
        //    Func<IActionResult>? redirct = null, 
        //    string? validMsg = null, string? errorMsg = null)
        //    => 
        //    // Gate 1 (Auth and Admin)
        //    Guard(requireAdmin: admin, requireAuth: auth) is { } r ? r : save != null 
        //    // Gate 2 POST
        //    ? await new Func<Task<IActionResult>>(async () => { var m = Build(table, title); var s = await save(); 
        //        if (!s) { m.Error = errorMsg; return View(view, m);}; 
        //        SetSuccess(s, validMsg ?? "done");
        //        return redirct == null ? RedirectToAction("Index") : redirct(); })() 

        //    // Gate 3 GET
        //    : await new Func<Task<IActionResult>>(async () => 
        //    { 
        //        var m = Build(table, title); 

        //        if (populate != null) 
        //            await populate(m); 
        //        if (check != null && !check(m)){ m.Error = errorMsg ?? "not found"; return NotFound(); }; 
        //        return View(view, m); })();

        /// <summary>
        /// Method to authorise if session data matches set requirements for user
        /// </summary>
        /// <param name="requireAdmin"> IfTrue Checks session for authenticated admin </param>
        /// <param name="requireAuth"> IfTrue Checks session for authenticated user </param>
        /// <returns> IActionResult redirect to login if session data doesnt match required params set, otherwise null </returns>
        protected IActionResult? Guard(bool requireAdmin = false, bool requireAuth = false) => (requireAuth || requireAdmin) && !IsAuth ? RedirectToAction("Login", "Account") : requireAdmin && !IsAdmin ? Forbid() : null;

        /// <summary>
        /// Method to pass information to view on redirect about status of operation
        /// </summary>
        /// <param name="saved"> determines if message is set </param>
        /// <param name="message"> message to be sent to page </param>
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

        private async Task SaveLog(bool isError, string view, string message)
        {
            string userName = HttpContext.Session.GetString("user") ?? "Guest";
            string role = HttpContext.Session.GetString("role") ?? "Guest";

            await _db.SaveLog(new Log
            {
                IsError = isError,
                UserName = userName,
                Role = role,
                View = view,
                Message = message,
                DateTime = DateTime.Now
            });
        }
    }
}