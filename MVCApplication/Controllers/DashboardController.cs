using Microsoft.AspNetCore.Mvc;
using MVCApplication.Models;

namespace MVCApplication.Controllers
{
    //this controller manages user profile viewing and editing.
    public class DashboardController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult MyBookings()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Profile()
        {
            return View();
        }
    }
}
