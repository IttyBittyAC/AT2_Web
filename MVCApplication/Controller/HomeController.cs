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
        public IActionResult Index()
        {
            var model = Build("home", "Home");
            return View(Home.Index, model);
        }

        [HttpGet("/Announcements")]
        public IActionResult Announcements()
        {
            var model = Build("announcements", "We have something to announce");
            return View(Home.Announcements, model);
        }

        [HttpGet("/Feedback")]
        public IActionResult Feedback()
        {
            var model = Build("feedback", "Feedback Form");
            return View(Home.Feedback, model);
        }

        [HttpPost("/Feedback")]
        public async Task<IActionResult> Feedback(Feedback feedback)
        {
            if (!ModelState.IsValid)
            {
                var model = Build("feedback", "Feedback");
                model.Error = "Please fill in all fields";
                model.Feedback = feedback;
                return View(Home.Feedback, model);
            }

            bool saved = await _db.SaveFeedback(feedback);
            SetSuccess(saved, "Thank you for making feedback");
            return RedirectToAction("Feedback");
        }

        [HttpGet("/FAQ")]
        public IActionResult FAQ()
        {
            var model = Build("faq", "FAQ");
            return View(Home.FAQ, model);
        }

        [HttpGet("/Confirmation")]
        public IActionResult Confirmation()
        {
            return View();
        }
    }
}