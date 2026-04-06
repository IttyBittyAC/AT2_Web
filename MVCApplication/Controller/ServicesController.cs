using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling service booking actions such as viewing bookings, booking services, and viewing booking details.
    /// </summary>
    public class ServicesController : BaseAppController
    {
        /// <summary>
        /// Initializes a new instance of the ServicesController class with the provided database context.
        /// </summary>
        /// <param name="db">Application database context</param>
        public ServicesController(AppDb db) : base(db)
        {
        }

        /// <summary>
        /// Displays all bookings or a specific booking if an ID is provided.
        /// </summary>
        /// <param name="id">Optional booking ID</param>
        /// <returns>Services view with booking data</returns>
        [HttpGet("/Services")]
        public Task<IActionResult> Index(int? id) => GraveMind(Services.Index, "bookings", "List of all bookings select one to view details", 
            populate: async m => { var (bs, b) = await _db.GetBooking(id); m.Bookings = bs ?? []; m.Booking = b;});

        /// <summary>
        /// Handles user actions from the services page such as navigating to details or booking pages.
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="action">Action to perform (Details or Book)</param>
        /// <returns>Redirects based on action or returns the view</returns>
        [HttpPost("/Services")]
        public IActionResult Index(int id, string action) => !string.IsNullOrEmpty(action) 
            ? action == "Details" 
                ? RedirectToAction("Details", new { Id = id }) 
                : action == "Book" ? RedirectToAction("Book") 
                    : NotFound() 
            : View(Services.Index);

        /// <summary>
        /// Displays details for a specific booking.
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking details view or error if not found</returns>
        [HttpGet("/Services/Details/{Id}")]
        public Task<IActionResult> Details(int id) => GraveMind(Services.Details, "bookings", "Services", populate: async m => { var (_, b) = await _db.GetBooking(id); m.Booking = b; }, check: m => m.Booking != null, errorMsg: "Nothing in bookings set");
        
        /// <summary>
        /// Displays the booking page for authenticated users.
        /// </summary>
        /// <returns>Booking form view</returns>
        [HttpGet("/Services/Book")]
        public Task<IActionResult> Book() => GraveMind(Services.Book, "bookings", "Book smt", auth : true);

        /// <summary>
        /// Handles booking submission by validating input and saving the booking to the database.
        /// </summary>
        /// <param name="booking">Booking object containing user input data</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Services/Book")]
        public Task<IActionResult> Book(Booking booking) => ModelState.IsValid 
            ? GraveMind(Services.Book, "bookings", "Book something", 
                save: async () => await _db.SaveBooking(booking), 
                validMsg: "Thank you for making a booking") 
            : GraveMind(Services.Book, "bookings", "Book something", 
                populate: async m => { m.Booking = booking; await Task.CompletedTask;}, 
                errorMsg: "Please fill in all fields");
    }
}