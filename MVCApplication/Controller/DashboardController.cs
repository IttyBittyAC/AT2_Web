using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.MessageDictionary;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// This controller is responsible for handling user dashboard-related actions, 
    /// such as displaying the main dashboard, user bookings, and profile information.
    /// </summary>
    public class DashboardController : BaseAppController<DashboardController>
    {
        /// <summary>
        /// Initializes a new instance of the DashboardController class with the provided database context.
        /// </summary>
        /// <param name="db">Application database context</param>
        /// <param name="logger">Logging Context for Controller Logger</param>
        public DashboardController(AppDb db, ILogger<DashboardController> logger) : base(db, logger)
        {
        }

        /// <summary>
        /// Displays the main dashboard view for authenticated users.
        /// </summary>
        /// <returns>Dashboard view</returns>
        [HttpGet("/Dashboard")]
        public Task<IActionResult> Index() => GraveMind(Dashboard.Index, MethodCode.DashBoardIndex, auth: true);

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
            MethodCode.DashBoardBooking,
            populate: async m =>
            {
                string? email = HttpContext.Session.GetString("user") ?? User.Identity?.Name;

                if (string.IsNullOrEmpty(email))
                {
                    m.Bookings = [];
                    return;
                }

                var (bs, _) = await _db.GetBookingByEmail(email);
                m.Bookings = bs ?? [];
            },
            auth: true
        );

        /// <summary>
        /// Displays the current logged-in user's profile information.
        /// </summary>
        /// <returns>Profile view with current user data</returns>
        [HttpGet("/Dashboard/Profile")]
        public Task<IActionResult> Profile() => GraveMind(
            Dashboard.Profile,
            MethodCode.DashBoardProfile,
            auth: true,
            populate: async m =>
            {
                string? email = HttpContext.Session.GetString("user") ?? User.Identity?.Name;

                if (string.IsNullOrWhiteSpace(email))
                {
                    m.User = null;
                    return;
                }

                var (_, user) = await _db.GetUserByEmail(email);
                m.User = user;

                var (bookings, _) = await _db.GetBookingByEmail(email);
                m.Bookings = bookings ?? [];
            },
            check: m => m.User != null
        );

        /// <summary>
        /// Updates the current logged-in user's profile information.
        /// </summary>
        /// <param name="updatedUser">Updated user details from the profile form</param>
        /// <returns>Redirects back to the profile page</returns>
        [HttpPost("/Dashboard/Profile")]
        public Task<IActionResult> Profile(User updatedUser) => GraveMind(
            Dashboard.Profile,
            MethodCode.DashBoardProfileUpdate,
            auth: true,
            save: async () =>
            {
                string? currentEmail = HttpContext.Session.GetString("user") ?? User.Identity?.Name;

                if (string.IsNullOrWhiteSpace(currentEmail))
                {
                    return false;
                }

                var (_, currentUser) = await _db.GetUserByEmail(currentEmail);

                if (currentUser == null)
                {
                    return false;
                }

                if (string.IsNullOrWhiteSpace(updatedUser.Username) ||
                    string.IsNullOrWhiteSpace(updatedUser.FullName) ||
                    string.IsNullOrWhiteSpace(updatedUser.Email))
                {
                    return false;
                }

                currentUser.Username = updatedUser.Username;
                currentUser.FullName = updatedUser.FullName;
                currentUser.Email = updatedUser.Email;

                int affected = await _db.UpdateUser(new List<User> { currentUser });

                if (affected > 0)
                {
                    HttpContext.Session.SetString("user", currentUser.Email);
                    return true;
                }

                return false;
            },
            redirect: () => RedirectToAction("Profile")
        );
    }
}