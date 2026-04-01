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
        public async Task<IActionResult> Index(int? id)
        {
            var model = Build("bookings", "List of all bookings select one to view details");
            var result = await _db.GetBooking(id);

            if (id == null)
            {
                model.Bookings = result.Item1 ?? new List<Booking>();
            }
            else
            {
                model.Booking = result.Item2;
            }

            return View(Services.Index, model);
        }

        [HttpPost("/Services")]
        public IActionResult Index(int id, string action)
        {
            if (action == "details")
            {
                return RedirectToAction("Details", new { id = id });
            }

            return RedirectToAction("Book");
        }

        [HttpGet("/Services/Details/{id}")]
        public async Task<IActionResult> Details(int id)
        {
            var model = Build("bookings", "Services");
            var result = await _db.GetBooking(id);

            if (result.Item2 == null)
            {
                return NotFound();
            }

            model.Booking = result.Item2;
            return View(Services.ServiceDetails, model);
        }

        [HttpGet("/Services/Book")]
        public IActionResult Book()
        {
            IActionResult? redirect = Guard();
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("bookings", "Book smt");
            return View(Services.Book, model);
        }

        [HttpPost("/Services/Book")]
        public async Task<IActionResult> Book(Booking booking)
        {
            if (!ModelState.IsValid)
            {
                var model = Build("bookings", "Book something");
                model.Error = "Please fill in all fields";
                model.Booking = booking;
                return View(Services.Book, model);
            }

            bool saved = await _db.SaveBooking(booking);
            SetSuccess(saved, "Thank you for making a booking");
            return RedirectToAction("Book");
        }
    }
}