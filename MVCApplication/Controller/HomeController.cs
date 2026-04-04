using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using System.Diagnostics;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    public class HomeController : BaseAppController
    {
        public HomeController(AppDb db) : base(db)
        {
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        [HttpGet("/Error")]
        public IActionResult Error()
        {
            return View(new ErrorViewModel
            {
                RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier
            });
        }

        [HttpGet("/")]
        [HttpGet("/Home")]
        public Task<IActionResult> Index() => GraveMind(Home.Index, "home", "Home");

        [HttpGet("/Announcements")]
        public Task<IActionResult> Announcements() => GraveMind(Home.Announcements, "announcements", "We have something to announce");

        [HttpGet("/Feedback")]
        public Task<IActionResult> Feedback() => GraveMind(Home.Feedback, "feedback", "Feedback Form");

        [HttpPost("/Feedback")]
        public Task<IActionResult> Feedback(Feedback feedback) => ModelState.IsValid
            ? GraveMind(Home.Feedback, "feedback", "Feedback",
                populate: async m => { m.Error = "All Fields are required"; await Task.CompletedTask; })
            : GraveMind(Home.Feedback, "feedback", "Feedback",
                save: () => _db.SaveFeedback(feedback),
                validMsg: "Successfully deleted user",
                redirct: () => RedirectToAction("Index"));         

        [HttpGet("/FAQ")]
        public Task<IActionResult> FAQ() => GraveMind(Home.FAQ, "faq", "FAQ");

        [HttpGet("/Confirmation")]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}