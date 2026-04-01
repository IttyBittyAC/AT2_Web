using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    public class DashboardController : BaseAppController
    {
        public DashboardController(AppDb db) : base(db)
        {
        }

        [HttpGet("/Dashboard")]
        public IActionResult Index()
        {
            IActionResult? redirect = Guard();
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("dashboard", "Dashboard");
            return View(Dashboard.Index, model);
        }

        [HttpGet("/Dashboard/MyBookings")]
        public async Task<IActionResult> MyBookings()
        {
            IActionResult? redirect = Guard();
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("bookings", "My Bookings");

            // If later you want to filter by current user, you'd do it here
            var result = await _db.GetBooking(null);
            model.Bookings = result.Item1 ?? new List<Booking>();

            return View(Dashboard.MyBookings, model);
        }

        [HttpGet("/Dashboard/Profile")]
        public IActionResult Profile()
        {
            IActionResult? redirect = Guard();
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("users", "Profile");
            return View(Dashboard.Profile, model);
        }
    }
}