using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// This controller is responsible for handling user dashboard-related actions, 
    /// such as displaying the main dashboard, user bookings, and profile information.
    /// </summary>
    public class DashboardController : BaseAppController
    {
        /// <summary>
        /// Initializes a new instance of the DashboardController class with the provided database context.
        /// </summary>
        /// <param name="db">Application database context</param>
        public DashboardController(AppDb db) : base(db)
        {
        }
        /// <summary>
        /// Displays the main dashboard view for authenticated users.
        /// </summary>
        /// <returns>Dashboard view</returns>
        [HttpGet("/Dashboard")]
        public Task<IActionResult> Index() => GraveMind(Dashboard.Index, "dashboard", "Dashboard", auth: true);

        // Need GetBookingByEmail db method to do fully implement this controller method
        [HttpGet("/Dashboard/MyBookings")]
        public Task<IActionResult> MyBookings() => GraveMind(Dashboard.MyBookings, "bookings", "My Bookings", 
            populate: async m => { var (bs, _) = await _db.GetBooking(null); m.Bookings = bs ?? []; }, 
            errorMsg: "No Bookings found of user", auth: true);

        // Using the GetUserByEmail db method to fetch user details for the profile view
        [HttpGet("/Dashboard/Profile")]
        public Task<IActionResult> Profile() => GraveMind(Dashboard.Profile, "users", "Profile", auth: true);

    }
}