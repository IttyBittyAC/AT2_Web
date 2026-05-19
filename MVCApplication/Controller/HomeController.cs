using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using System.Diagnostics;
using static MVCApplication.Helpers.V;
using static MVCApplication.Helpers.MessageDictionary;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling general site pages such as the home page, announcements, feedback, FAQ, and error pages.
    /// </summary>
    public class HomeController : BaseAppController<HomeController>
    {
        /// <summary>
        /// Initializes a new instance of the HomeController class with the provided database context.
        /// </summary>
        /// <param name="db">Application database context</param>
        /// <param name="logger">Logging Context for Controller Logger</param>
        public HomeController(AppDb db, ILogger<HomeController> logger) : base(db, logger)
        {
        }

        /// <summary>
        /// Displays the error page with the current request identifier.
        /// </summary>
        /// <returns>Error view with request details</returns>
        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("/Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        /// <summary>
        /// Displays the home page.
        /// Populates announcements and events so they render on the index view.
        /// </summary>
        /// <returns>Home view</returns>
        [HttpGet("/")]
        [HttpGet("/Home")]
        public Task<IActionResult> Index() => GraveMind(
            Home.Index,
            MethodCode.HomeIndex,
            populate: async m =>
            {
                // populate upcoming events
                var (events, _) = await _db.GetEvent(null);
                m.Events = events ?? [];

                // populate announcements
                m.Announcements = await _db.GetAnnouncements() ?? [];
            });

        /// <summary>
        /// Displays the announcements page with announcement data from the database.
        /// </summary>
        /// <returns>Announcements view with data</returns>
        [HttpGet("/Announcements")]
        public Task<IActionResult> Announcements() => GraveMind(
            Home.Announcements,
            MethodCode.HomeAnnouncements,
            populate: async m =>
            {
                m.Announcements = await _db.GetAnnouncements() ?? [];
            });

        /// <summary>
        /// Displays the feedback form page.
        /// </summary>
        /// <returns>Feedback form view</returns>
        [HttpGet("/Feedback")]
        public Task<IActionResult> Feedback() => GraveMind(Home.Feedback, MethodCode.HomeFeedBack);

        /// <summary>
        /// Handles feedback form submission by validating input and saving the feedback to the database.
        /// </summary>
        /// <param name="feedback">Feedback object containing user input data</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Feedback")]
        public Task<IActionResult> Feedback(Feedback feedback) => !ModelState.IsValid
            ? GraveMind(Home.Feedback, MethodCode.HomeFeedBackInvalid,
                populate: async m => { m.Error = Store[MethodCode.HomeFeedBackInvalid].ErrorMsg;
                    await Task.CompletedTask; })
            : GraveMind(Home.Feedback, MethodCode.HomeFeedBack,
                save: () => _db.SaveFeedback(feedback),
                redirect: () => RedirectToAction("Index"));


        /// <summary>
        /// Displays the FAQ page.
        /// </summary>
        /// <returns>FAQ view</returns>
        [HttpGet("/FAQ")]
        public Task<IActionResult> FAQ() => GraveMind(Home.FAQ, MethodCode.HomeFAQ);

        /// <summary>
        /// Displays the confirmation page.
        /// </summary>
        /// <returns>Confirmation view</returns>
        [HttpGet("/Confirmation")]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}