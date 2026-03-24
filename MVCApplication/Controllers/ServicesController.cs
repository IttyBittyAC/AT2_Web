using Microsoft.AspNetCore.Mvc;

namespace MVCApplication.Controllers
{
    public class ServicesController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

        public IActionResult ServiceDetails(int id)
        {
            return View();
        }

        public IActionResult Book()
        {
            return View();
        }
    }
}
