using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;
using static MVCApplication.Helpers.MessageDictionary;
using static MVCApplication.Helpers.ActionEnums;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling service booking actions such as viewing bookings, booking services, and viewing booking details.
    /// </summary>
    public class ServicesController : BaseAppController<ServicesController>
    {
        /// <summary>
        /// Initializes a new instance of the ServicesController class with the provided database context.
        /// </summary>
        /// <param name="db">Application database context</param>
        /// <param name="logger">Logging Context for Controller Logger</param>
        public ServicesController(AppDb db, ILogger<ServicesController> logger) : base(db, logger)
        {
        }

        /// <summary>
        /// Displays all bookings or a specific booking if an ID is provided.
        /// </summary>
        /// <param name="id">Optional booking ID</param>
        /// <returns>Services view with booking data</returns>
        [HttpGet("/Services")]
        public Task<IActionResult> Index(int? id) => GraveMind(Services.Index, MethodCode.ServiceIndex,
            populate: async m => { var (bs, b) = await _db.GetBooking(id); m.Bookings = bs ?? []; m.Booking = b; });

        /// <summary>
        /// Handles user actions from the services page such as navigating to details or booking pages.
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <param name="action">Action to perform (Details or Book)</param>
        /// <returns>Redirects based on action or returns the view</returns>
        [HttpPost("/Services")]
        public Task<IActionResult> Index(int id, ServiceAction? action) => action == null
        ? GraveMind(Services.Index,
            MethodCode.ServiceIndex)
        : action switch
        {
            ServiceAction.Details => GraveMind(Services.Index,
                MethodCode.ServiceDetail,
                save: () => Task.FromResult(true),redirect: () => RedirectToAction("Details", new { id })),

            ServiceAction.Book => GraveMind(Services.Index,
                MethodCode.ServiceBook,
                save: () => Task.FromResult(true),redirect: () => RedirectToAction("Book")),
            _ => Task.FromResult<IActionResult>(NotFound())
        };

        /// <summary>
        /// Displays details for a specific booking.
        /// </summary>
        /// <param name="id">Booking ID</param>
        /// <returns>Booking details view or error if not found</returns>
        [HttpGet("/Services/Details/{Id}")]
        public Task<IActionResult> Details(int id) => GraveMind(Services.Details, MethodCode.ServiceDetail, populate: async m => { var (_, b) = await _db.GetBooking(id); m.Booking = b; }, check: m => m.Booking != null);
        
        /// <summary>
        /// Displays the booking page for authenticated users.
        /// </summary>
        /// <returns>Booking form view</returns>
        [HttpGet("/Services/Book")]
        public Task<IActionResult> Book() => GraveMind(Services.Book,MethodCode.ServiceBook, auth : true);

        /// <summary>
        /// Handles booking submission by validating input and saving the booking to the database.
        /// </summary>
        /// <param name="booking">Booking object containing user input data</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Services/Book")]
        public Task<IActionResult> Book(Booking booking) => ModelState.IsValid 
            ? GraveMind(Services.Book, MethodCode.ServiceBook, 
                save: async () => await _db.SaveBooking(booking)) 
            : GraveMind(Services.Book, MethodCode.ServiceBookInvalid, 
                populate: async m => { m.Booking = booking; await Task.CompletedTask;});
    }
}