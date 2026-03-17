using Microsoft.AspNetCore.Mvc;

namespace MVCApplication.Controllers
{
    //this controller manages displaying of events and event details.
    public class EventsController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(int id)
        {
            return View();
        }
    }
}
