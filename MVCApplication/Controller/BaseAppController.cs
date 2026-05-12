using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.UserRole;
using static MVCApplication.Helpers.MessageDictionary;
using MVCApplication.Helpers;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Base controller that provides shared database access and common functionality for all other controllers in the application.
    /// </summary>
    public abstract class BaseAppController<T> : Controller
    {
        /// <summary>
        /// Database context for accessing application data, shared across all controllers that inherit from BaseAppController.
        /// </summary>
        protected readonly AppDb _db;
        private readonly ILogger<T> _logger;
        /// <summary>
        /// Initializes a new instance of the BaseAppController class with the provided database context, 
        /// allowing derived controllers to access the database through the _db field.
        /// </summary>
        /// <param name="db"></param>
        /// <param name="logger"></param>
        protected BaseAppController(AppDb db, ILogger<T> logger)
        {
            _db = db;
            _logger = logger;
        }

        /// <summary>
        /// Gets authentication status of session and sets true if not null
        /// </summary>
        protected bool IsAuth
        {
            get
            {
                return HttpContext.Session.GetString(SessionKeys.Type) != null;
            }
        }

        /// <summary>
        /// Gets role of session and sets true if admin
        /// </summary>
        protected bool IsAdmin
        {
            get
            {
                return HttpContext.Session.GetString(SessionKeys.Role) == admin.ToString();
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
                HttpContext.Session.GetString(SessionKeys.Type),
                HttpContext.Session.GetString(SessionKeys.Role),
                title,
                returnUrl
            );
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <param name="operation"></param>
        /// <param name="e"></param>
        /// <param name="s"></param>
        /// <returns></returns>
        private async Task<IActionResult> logWrapper(string path, string? e, string? s, Func<Task<IActionResult>> operation) => 
            await new Func<Task<IActionResult>>(async () => { 
                var sw = Stopwatch.StartNew(); 
                try { var result = await operation(); sw.Stop(); 
                    await( result switch 
                    { 
                        NotFoundResult => SaveLog(true, path, $"FAIL: TRACE: {HttpContext.TraceIdentifier} | TYPE: {result.GetType().Name} | TIME: {sw.ElapsedMilliseconds}ms | USER MSG:{e}"), 
                        ForbidResult => SaveLog(true, path, $"FAIL: TRACE: {HttpContext.TraceIdentifier} | TYPE: {result.GetType().Name} | TIME: {sw.ElapsedMilliseconds}ms | USER MSG:{e}"), 
                        RedirectResult =>  SaveLog(false, path, $"PASS: TRACE: {HttpContext.TraceIdentifier} | TIME: {sw.ElapsedMilliseconds}ms | USER MSG:{s}"), 
                        ViewResult =>SaveLog(false, path, $"PASS: TRACE: {HttpContext.TraceIdentifier} | TIME: {sw.ElapsedMilliseconds}ms | USER MSG:{s}"), 
                        _ => SaveLog(false, path, $"PASS: TRACE: {HttpContext.TraceIdentifier} | TIME: {sw.ElapsedMilliseconds}ms | USER MSG:{s}"), }); 
                    return result; } 
                catch (Exception ex) { 
                    sw.Stop(); 
                    await SaveLog(true, path, $"EXCEPTION: TRACE {HttpContext.TraceIdentifier} | TYPE: {ex.GetType().Name} | TIME: {sw.ElapsedMilliseconds}ms {ex.Message}");
                    _logger.LogError(ex, "ERROR in Gravemind");
                    throw; } })();
        /// <summary>
        /// Method that handles
        ///     Authorisation (auth/admin)
        ///     GET model pop and validation
        ///     POST save and redirection
        ///     
        /// We Trade One Villain for Another 
        /// </summary>
        /// <param name="view"> Page to be displayed (Required) </param>
        /// <param name="controllerOp">Key of Dict containing value pair of UX parameters</param>
        /// <param name="auth"> If True, required authenticated user </param>
        /// <param name="admin"> If True, required authenticated admin </param>
        /// <param name="populate"> Optional delegate that populates the view model with data from db before rendering the view </param>
        /// <param name="check"> Optional delegate that validates model and returns false to trigger 404 </param>
        /// <param name="save"> Optional delegate that does POST operations </param>
        /// <param name="redirect"> Optional delegate that stores where to be redirected to after a POST </param>
        /// <returns> An IActionResult representing either a view, redirect, or error response </returns>

        protected Task<IActionResult> GraveMind(string view, MethodCode controllerOp,
            bool auth = false, bool admin = false,
            Func<AndInTheDarknessBindThem, Task>? populate = null,
            Func<AndInTheDarknessBindThem, bool>? check = null,
            Func<Task<bool>>? save = null,
            Func<IActionResult>? redirect = null)
            => 
            logWrapper(HttpContext.Request.Path.Value ?? view, e: Store[controllerOp].ErrorMsg, s: Store[controllerOp].SuccessMsg, 
                async() =>           
                    // Gate 1 (Auth and Admin)
                    Guard(requireAdmin: admin, requireAuth: auth) is { } r 
                    ? r : save != null
                    // Gate 2 POST
                    ? await new Func<Task<IActionResult>>(async () =>
                    {
                        var op = Store[controllerOp];
                        var m = Build(op.Table, op.Title);
                        var s = await save();
                        
                        if (!s)
                        {
                            m.Error = op.ErrorMsg;
                            return View(view, m);
                        }
                        TempData["Success"] = op.SuccessMsg ?? "Completed(No Message Specified)";

                        return redirect == null ? RedirectToAction("Index") : redirect();
                    })()

                    // Gate 3 GET
                    : await new Func<Task<IActionResult>>(async () =>
                    {
                        var op = Store[controllerOp];
                        var m = Build(op.Table, op.Title);

                        if (populate != null)
                        {
                            await populate(m);
                        }

                        if (check != null && !check(m))
                        {
                            m.Error = op.ErrorMsg ?? "ERROR: Operation Failed";
                            return NotFound();
                        }
                        m.Success = TempData["Success"] as string;
                        TempData["Success"] = null;
                        return View(view, m);
                    })());
        /// <summary>
        /// Method to authorise if session data matches set requirements for user
        /// </summary>
        /// <param name="requireAdmin"> IfTrue Checks session for authenticated admin </param>
        /// <param name="requireAuth"> IfTrue Checks session for authenticated user </param>
        /// <returns> IActionResult redirect to login if session data doesnt match required params set, otherwise null </returns>
        protected IActionResult? Guard(bool requireAdmin = false, bool requireAuth = false) => (requireAuth || requireAdmin) && !IsAuth ? RedirectToAction("Login", "Account") : requireAdmin && !IsAdmin ? new StatusCodeResult(403) : null;

        private async Task SaveLog(bool isError, string view, string message)
        {
            try
            {
                string userName = HttpContext.Session.GetString(SessionKeys.Type) ?? "Guest";
                string role = HttpContext.Session.GetString(SessionKeys.Role) ?? "Guest";

                await _db.SaveLog(new Log
                {
                    IsError = isError,
                    UserName = userName,
                    Role = role,
                    View = view,
                    Message = message,
                    DateTime = DateTime.UtcNow,
                });
            }
            catch(Exception ex) 
            {
                Console.Error.WriteLine($"[LOG FAILED] {ex.Message}");
                _logger.LogError(ex, "[LOG FAILED]");
            }
        }
    }
}