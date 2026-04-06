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

        /// <summary>
        /// Handles HTTP GET requests for the My Bookings dashboard view, retrieving and displaying bookings associated
        /// with the currently authenticated user.
        /// </summary>
        /// <remarks>This action requires the user to be authenticated. If the user is not authenticated
        /// or has no associated bookings, the view will display an empty bookings list.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>
        /// that renders the My Bookings view with the user's bookings.</returns>
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

        /// <summary>
        /// Handles HTTP GET requests for the user profile dashboard view, retrieving and displaying the profile
        /// information for the currently authenticated user.
        /// </summary>
        /// <remarks>The user must be authenticated to access this endpoint. If the user is not
        /// authenticated or cannot be found, an error message is displayed instead of the profile
        /// information.</remarks>
        /// <returns>A task that represents the asynchronous operation. The task result contains an <see cref="IActionResult"/>
        /// that renders the profile view for the authenticated user, or an error view if the user is not found or not
        /// authenticated.</returns>
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