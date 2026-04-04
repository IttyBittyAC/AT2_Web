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
        public Task<IActionResult> Index() => GraveMind(Dashboard.Index, "dashboard", "Dashboard", auth: true);

        // Need GetByEmailforTables db method to do fully implement this controller method
        [HttpGet("/Dashboard/MyBookings")]
        public Task<IActionResult> MyBookings() => GraveMind(Dashboard.MyBookings, "bookings", "My Bookings", 
            populate: async m => { var (bs, _) = await _db.GetBooking(null); m.Bookings = bs ?? []; }, 
            errorMsg: "No Bookings found of user", auth: true);

        // Need GetByEmail db method to do fully implement this controller method
        [HttpGet("/Dashboard/Profile")]
        public Task<IActionResult> Profile() => GraveMind(Dashboard.Profile, "users", "Profile", auth: true);

    }
}