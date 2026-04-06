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

        // Using the GetBookingByEmail db method to fetch user-specific bookings for the My Bookings view
        [HttpGet("/Dashboard/MyBookings")]
        public Task<IActionResult> MyBookings() => GraveMind(
            Dashboard.MyBookings,
            "bookings",
            "My Bookings",
            populate: async m =>
            {
                var email = User.Identity?.Name;

                if (string.IsNullOrEmpty(email))
                {
                    m.Bookings = [];
                    return;
                }

                var (bs, _) = await _db.GetBookingByEmail(email);
                m.Bookings = bs ?? [];
            },
            errorMsg: "No Bookings found of user",
            auth: true
        );

        // Using the GetUserByEmail db method to fetch user details for the profile view
        [HttpGet("/Dashboard/Profile")]
        public Task<IActionResult> Profile() => GraveMind(
            Dashboard.Profile,
            "users",
            "Profile",
            populate: async m =>
            {
                var email = User.Identity?.Name;

                if (string.IsNullOrEmpty(email))
                {
                    m.User = null;
                    return;
                }

                var (_, user) = await _db.GetUserByEmail(email);
                m.User = user;
            },
            errorMsg: "User not found",
            auth: true
        );
    }
}