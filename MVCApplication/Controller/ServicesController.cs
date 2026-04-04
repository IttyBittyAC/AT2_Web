using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    public class ServicesController : BaseAppController
    {
        public ServicesController(AppDb db) : base(db)
        {
        }

        [HttpGet("/Services")]
        public Task<IActionResult> Index(int? id) => GraveMind(Services.Index, "bookings", "List of all bookings select one to view details", 
            populate: async m => { var (bs, b) = await _db.GetBooking(id); m.Bookings = bs ?? []; m.Booking = b;});

        [HttpPost("/Services")]
        public IActionResult Index(int id, string action) => !string.IsNullOrEmpty(action) 
            ? action == "Details" 
                ? RedirectToAction("Details", new { Id = id }) 
                : action == "Book" ? RedirectToAction("Book") 
                    : NotFound() 
            : View(Services.Index);

        [HttpGet("/Services/Details/{Id}")]
        public Task<IActionResult> Details(int id) => GraveMind(Services.Details, "bookings", "Services", populate: async m => { var (_, b) = await _db.GetBooking(id); m.Booking = b; }, check: m => m.Booking != null, errorMsg: "Nothing in bookings set");

        [HttpGet("/Services/Book")]
        public Task<IActionResult> Book() => GraveMind(Services.Book, "bookings", "Book smt", auth : true);

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