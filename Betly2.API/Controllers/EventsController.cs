using Betly.core.Interfaces;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Betly2.API.Controllers
{
    [ApiController]
    [Route("api/events")]
    public class EventsController : ControllerBase
    {
        private readonly IEventRepository _eventRepository;
        private readonly Betly.core.Services.BettingService _bettingService;

        public EventsController(IEventRepository eventRepository, Betly.core.Services.BettingService bettingService)
        {
            _eventRepository = eventRepository;
            _bettingService = bettingService;
        }

        [HttpPost("{id}/resolve")]
        public async Task<IActionResult> ResolveEvent(int id, [FromBody] Betly.core.DTOs.ResolveEventRequest request)
        {
             try
            {
                var eventItem = await _eventRepository.GetEventByIdAsync(id);
                if (eventItem == null) return NotFound(new { message = "Event not found" });

                // Check authorization using the OwnerId passed in the request body
                // In a production scenario, this should be validated against a trusted token or context.
                if (request.OwnerId != eventItem.OwnerId)
                {
                    return Unauthorized(new { message = "Only the event owner can resolve this event." });
                }

                await _bettingService.ResolveEventAsync(id, request.WinningOutcome);
                return Ok(new { message = "Event resolved successfully" });
            }
            catch (System.Exception ex)
            {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpGet]
        public async Task<IActionResult> GetAllEvents()
        {
            var events = await _eventRepository.GetAllEventsAsync();
            return Ok(events);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetEventById(int id)
        {
            var eventItem = await _eventRepository.GetEventByIdAsync(id);
            if (eventItem == null)
                return NotFound();

            return Ok(eventItem);
        }
        [HttpPost]
        public async Task<IActionResult> CreateEvent([FromBody] Betly.core.Models.Event eventItem)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var createdEvent = await _eventRepository.AddEventAsync(eventItem);
            return CreatedAtAction(nameof(GetEventById), new { id = createdEvent.Id }, createdEvent);
        }

        [HttpGet("my-events")]
        public async Task<IActionResult> GetMyEvents()
        {
             var userIdStr = User.FindFirst("UserId")?.Value;
             if (string.IsNullOrEmpty(userIdStr)) return Unauthorized();

             int userId = int.Parse(userIdStr);
             var events = await _eventRepository.GetEventsByOwnerAsync(userId);
             return Ok(events);
        }

        [HttpGet("owner/{ownerId}")]
        public async Task<IActionResult> GetEventsByOwnerId(int ownerId)
        {
             // In a real app, verify the requestor has permission to view these, but for now public or admin use.
             var events = await _eventRepository.GetEventsByOwnerAsync(ownerId);
             return Ok(events);
        }
    }
}
