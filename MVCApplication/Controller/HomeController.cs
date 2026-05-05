using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using System.Diagnostics;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling general site pages such as the home page, announcements, feedback, FAQ, and error pages.
    /// </summary>
    public class HomeController : BaseAppController
    {
        /// <summary>
        /// Initializes a new instance of the HomeController class with the provided database context.
        /// </summary>
        /// <param name="db">Application database context</param>
        public HomeController(AppDb db) : base(db)
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
        /// </summary>
        /// <returns>Home view</returns>
        [HttpGet("/")]
        [HttpGet("/Home")]
        public Task<IActionResult> Index() => GraveMind(Home.Index, "home", "Home");

        /// <summary>
        /// Displays the announcements page.
        /// </summary>
        /// <returns>Announcements view</returns>
        [HttpGet("/Announcements")]
        public Task<IActionResult> Announcements() => GraveMind(Home.Announcements, "announcements", "We have something to announce");

        /// <summary>
        /// Displays the feedback form page.
        /// </summary>
        /// <returns>Feedback form view</returns>
        [HttpGet("/Feedback")]
        public Task<IActionResult> Feedback() => GraveMind(Home.Feedback, "feedback", "Feedback Form");

        /// <summary>
        /// Handles feedback form submission by validating input and saving the feedback to the database.
        /// </summary>
        /// <param name="feedback">Feedback object containing user input data</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Feedback")]
        public Task<IActionResult> Feedback(Feedback feedback) => ModelState.IsValid
            ? GraveMind(Home.Feedback, "feedback", "Feedback",
                populate: async m => { m.Error = "All Fields are required"; await Task.CompletedTask; })
            : GraveMind(Home.Feedback, "feedback", "Feedback",
                save: () => _db.SaveFeedback(feedback),
                validMsg: "Successfully deleted user",
                redirct: () => RedirectToAction("Index"));

        /// <summary>
        /// Displays the FAQ page.
        /// </summary>
        /// <returns>FAQ view</returns>
        [HttpGet("/FAQ")]
        public Task<IActionResult> FAQ() => GraveMind(Home.FAQ, "faq", "FAQ");

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