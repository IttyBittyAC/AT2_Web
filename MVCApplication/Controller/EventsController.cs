using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    public class EventsController : BaseAppController
    {
        public EventsController(AppDb db) : base(db)
        {
        }

        [HttpGet("/Events")]
        public async Task<IActionResult> Index()
        {
            var model = Build("events", "Events");
            var result = await _db.GetEvent(null);
            model.Events = result.Item1 ?? new List<Event>();

            return View(Events.Index, model);
        }

        [HttpGet("/Events/Create")]
        public IActionResult Create()
        {
            IActionResult? redirect = Guard();
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("events", "Make an event");
            return View(Events.Create, model);
        }

        [HttpPost("/Events/Create")]
        public async Task<IActionResult> Create(Event singleEvent)
        {
            IActionResult? redirect = Guard();
            if (redirect != null)
            {
                return redirect;
            }

            if (!ModelState.IsValid)
            {
                var model = Build("events", "Events");
                model.Error = "Please fill in all fields";
                model.Event = singleEvent;
                return View(Events.Create, model);
            }

            bool saved = await _db.SaveEvent(singleEvent);
            SetSuccess(saved, "Made event successfully");
            return RedirectToAction("Create");
        }

        [HttpGet("/Events/Details")]
        public IActionResult Details()
        {
            IActionResult? redirect = Guard();
            if (redirect != null)
            {
                return redirect;
            }

            var model = Build("events", "Event details");
            return View(Events.EventDetails, model);
        }

        [HttpPost("/Events/Details")]
        public async Task<IActionResult> Details(int? id)
        {
            var model = Build("events", "Display Events/Event");
            var result = await _db.GetEvent(id);

            if (id == null)
            {
                model.Events = result.Item1 ?? new List<Event>();
            }
            else
            {
                model.Event = result.Item2;
            }

            return View(Events.EventDetails, model);
        }
    }
}