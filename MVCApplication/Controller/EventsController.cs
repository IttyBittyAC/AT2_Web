using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Models;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling event-related actions such as viewing, creating, and displaying event details.
    /// </summary>
    public class EventsController : BaseAppController
    {
        /// <summary>
        /// Initializes a new instance of the EventsController class with the provided database context.
        /// </summary>
        /// <param name="db">Application database context</param>
        public EventsController(AppDb db) : base(db)
        {
        }

        /// <summary>
        /// Displays all events and populates the model with event data from the database.
        /// </summary>
        /// <returns>Events view with data</returns>
        [HttpGet("/Events")]
        public Task<IActionResult> Index() => GraveMind(Events.Index, "events", "Events", 
            populate: async m => { var (events, _) = await _db.GetEvent(null); m.Events = events ?? [];});

        /// <summary>
        /// Displays the event creation page for authenticated users.
        /// </summary>
        /// <returns>Create event view</returns>
        [HttpGet("/Events/Create")]
        public Task<IActionResult> Create() => GraveMind(Events.Create, "events", "Make an event", auth: true );

        /// <summary>
        /// Handles event creation by validating input and saving the new event to the database.
        /// </summary>
        /// <param name="singleEvent">Event object containing user input data</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Events/Create")]
        public Task<IActionResult> Create(Event singleEvent) => !ModelState.IsValid 
            ? GraveMind(Events.Create, "events", "Make an event", auth: true, 
                populate: async m => { m.Event = singleEvent; await Task.CompletedTask;}, 
                errorMsg: "Please fill in all fields")  
            : GraveMind(Events.Create, auth: true,  
                validMsg: "Made event successfully", 
                save: () =>  _db.SaveEvent(singleEvent), 
                redirct: () => RedirectToAction("Details"));

        /// <summary>
        /// Displays the event details page.
        /// </summary>
        /// <returns>Event details view</returns>
        [HttpGet("/Events/Details")]
        public Task<IActionResult> Details() => GraveMind(Events.Details, "events", "Event details");

        /// <summary>
        /// Retrieves and displays specific event details based on the provided ID.
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Event details view with data or error if not found</returns>
        [HttpPost("/Events/Details")]
        public Task<IActionResult> Details(int? id) => GraveMind(Events.Details, "events", "Display Events/Event", populate: async m => { var (events, _event) = await _db.GetEvent(id); m.Events = events ?? []; m.Event = _event; }, errorMsg: "Event not found");
    }
}