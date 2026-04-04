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
        public Task<IActionResult> Index() => GraveMind(Events.Index, "events", "Events", 
            populate: async m => { var (events, _) = await _db.GetEvent(null); m.Events = events ?? [];});

        [HttpGet("/Events/Create")]
        public Task<IActionResult> Create() => GraveMind(Events.Create, "events", "Make an event", auth: true );

        [HttpPost("/Events/Create")]
        public Task<IActionResult> Create(Event singleEvent) => !ModelState.IsValid 
            ? GraveMind(Events.Create, "events", "Make an event", auth: true, 
                populate: async m => { m.Event = singleEvent; await Task.CompletedTask;}, 
                errorMsg: "Please fill in all fields")  
            : GraveMind(Events.Create, auth: true,  
                validMsg: "Made event successfully", 
                save: () =>  _db.SaveEvent(singleEvent), 
                redirct: () => RedirectToAction("Details"));

        [HttpGet("/Events/Details")]
        public Task<IActionResult> Details() => GraveMind(Events.Details, "events", "Event details");

        [HttpPost("/Events/Details")]
        public Task<IActionResult> Details(int? id) => GraveMind(Events.Details, "events", "Display Events/Event", populate: async m => { var (events, _event) = await _db.GetEvent(id); m.Events = events ?? []; m.Event = _event; }, errorMsg: "Event not found");
    }
}