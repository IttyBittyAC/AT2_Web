using Microsoft.AspNetCore.Mvc;
using MVCApplication.Data;
using MVCApplication.Helpers;
using MVCApplication.Models;
using static MVCApplication.Helpers.MessageDictionary;
using static MVCApplication.Helpers.V;

namespace MVCApplication.Controllers
{
    /// <summary>
    /// Controller responsible for handling event-related actions such as viewing, creating, and displaying event details.
    /// </summary>
    public class EventsController : BaseAppController<EventsController>
    {
        /// <summary>
        /// Initializes a new instance of the EventsController class with the provided database context.
        /// </summary>
        /// <param name="logger">Logging Context for Controller Logger</param>
        /// <param name="db">Application database context</param>
        public EventsController(AppDb db, ILogger<EventsController> logger) : base(db,logger)
        {
        }

        /// <summary>
        /// Displays all events and populates the model with event data from the database.
        /// </summary>
        /// <returns>Events view with data</returns>
        [HttpGet("/Events")]
        public Task<IActionResult> Index() => GraveMind(Events.Index, MethodCode.EventsIndex, 
            populate: async m => { var (events, _) = await _db.GetEvent(null); m.Events = events ?? [];});

        /// <summary>
        /// Displays the event creation page for authenticated users.
        /// </summary>
        /// <returns>Create event view</returns>
        [HttpGet("/Events/Create")]
        public Task<IActionResult> Create() => GraveMind(Events.Create, MethodCode.EventsCreate, auth: true);

        /// <summary>
        /// Handles event creation by validating input and saving the new event to the database.
        /// </summary>
        /// <param name="singleEvent">Event object containing user input data</param>
        /// <param name="id">Id to be redirected to</param>
        /// <returns>Redirects on success or returns the view with errors</returns>
        [HttpPost("/Events/Create")]
        public Task<IActionResult> Create(Event singleEvent, int? id) => !ModelState.IsValid 
            ? GraveMind(Events.Create, MethodCode.EventsCreateInvalid, auth: true, 
                populate: async m => { m.Event = singleEvent; await Task.CompletedTask;})  
            : GraveMind(Events.Create, MethodCode.EventsCreate,
                auth: true, 
                save:  async () => { (bool l,int i) = await _db.SaveEvent(singleEvent); id = i; return l;  }, 
                redirect: () => RedirectToAction($"Details", new {id}));

        /// <summary>
        /// Displays the event details page.
        /// </summary>
        /// <returns>Event details view</returns>
        [HttpGet("/Events/Details")]
        public Task<IActionResult> Details() => GraveMind(Events.Details, MethodCode.EventsDetails);

        /// <summary>
        /// Retrieves and displays specific event details based on the provided ID.
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Event details view with data or error if not found</returns>
        [HttpGet("/Events/Details/{id}")]
        public Task<IActionResult> Details(int id) => 
            GraveMind(Events.Details, MethodCode.EventsDetails, 
                populate: async m => { 
                    var (_, _event) = await _db.GetEvent(id); 
                    m.Event = _event; 
                }, 
                check: m => m.Event != null);

        /// <summary>
        /// Registers the current logged-in user for an event.
        /// </summary>
        /// <param name="id">Event ID</param>
        /// <returns>Redirects back to the event details page</returns>
        [HttpPost("/Events/Register/{id}")]
        public Task<IActionResult> Register(int id) => GraveMind(
            Events.Details,
            MethodCode.EventsRegister,
            auth: true,
            save: async () =>
            {
                string? email = HttpContext.Session.GetString(SessionKeys.Type);

                if (string.IsNullOrWhiteSpace(email))
                {
                    return false;
                }

                return await _db.SaveEventBooking(id, email);
            },
            redirect: () => RedirectToAction("Details", new { id }));
    }


}