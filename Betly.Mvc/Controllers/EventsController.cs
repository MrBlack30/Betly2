using Betly.core.DTOs;
using Betly.core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http.Json;

namespace Betly.Mvc.Controllers
{
    public class EventsController : Controller
    {
        private const string ApiBaseUrl = "https://localhost:7203";
        private readonly HttpClient _httpClient;

        public EventsController()
        {
            _httpClient = new HttpClient();
        }

        public async Task<IActionResult> Index()
        {
            try
            {
                var events = await _httpClient.GetFromJsonAsync<List<Event>>($"{ApiBaseUrl}/api/events");
                return View(events);
            }
            catch
            {
                ModelState.AddModelError("", "Unable to load events.");
                return View(new List<Event>());
            }
        }

        public async Task<IActionResult> PlaceBet(int id)
        {
            // Get Event details to show
            try
            {
                var eventItem = await _httpClient.GetFromJsonAsync<Event>($"{ApiBaseUrl}/api/events/{id}");
                if (eventItem == null) return NotFound();

                ViewBag.Event = eventItem;
                return View(new BetRequest { EventId = id });
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBet(BetRequest model)
        {
            // UserId must be set from claims
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");

            model.UserId = int.Parse(userIdStr);

            if (!ModelState.IsValid)
            {
                // Re-fetch event for display
                 try
                {
                    var eventItem = await _httpClient.GetFromJsonAsync<Event>($"{ApiBaseUrl}/api/events/{model.EventId}");
                    ViewBag.Event = eventItem;
                }
                catch { }
                return View(model);
            }

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/api/bets", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Bet placed successfully!";
                    return RedirectToAction("Dashboard", "Account");
                }
                else
                {
                    var error = await response.Content.ReadFromJsonAsync<dynamic>(); // Or use Dictionary
                    ModelState.AddModelError("", "Failed to place bet. " + error);
                    
                    // Re-fetch event for display
                     try
                    {
                        var eventItem = await _httpClient.GetFromJsonAsync<Event>($"{ApiBaseUrl}/api/events/{model.EventId}");
                        ViewBag.Event = eventItem;
                    }
                    catch { }
                    return View(model);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Error communicating with server.");
                 // Re-fetch event for display
                 try
                {
                    var eventItem = await _httpClient.GetFromJsonAsync<Event>($"{ApiBaseUrl}/api/events/{model.EventId}");
                    ViewBag.Event = eventItem;
                }
                catch { }
                return View(model);
            }
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> Create(Event model)
        {
            if (!ModelState.IsValid)
                return View(model);

            try
            {
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/api/events", model);
                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Event created successfully!";
                    return RedirectToAction("Index");
                }
                else
                {
                    var errorContent = await response.Content.ReadAsStringAsync();
                    ModelState.AddModelError("", $"Failed to create event. Status: {response.StatusCode}. Details: {errorContent}");
                    return View(model);
                }
            }
            catch
            {
                ModelState.AddModelError("", "Service unavailable.");
                return View(model);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Resolve(int id)
        {
            try
            {
                var eventItem = await _httpClient.GetFromJsonAsync<Event>($"{ApiBaseUrl}/api/events/{id}");
                if (eventItem == null) return NotFound();
                return View(eventItem);
            }
            catch
            {
                return RedirectToAction("Index");
            }
        }

        [HttpPost]
        public async Task<IActionResult> Resolve(int EventId, string WinningOutcome)
        {
            if (string.IsNullOrEmpty(WinningOutcome))
            {
                ModelState.AddModelError("", "Please select a winning outcome.");
                 // Re-fetch event for display
                try
                {
                    var eventItem = await _httpClient.GetFromJsonAsync<Event>($"{ApiBaseUrl}/api/events/{EventId}");
                    return View(eventItem);
                }
                catch { return RedirectToAction("Index"); }
            }

            try
            {
                var request = new ResolveEventRequest { WinningOutcome = WinningOutcome };
                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/api/events/{EventId}/resolve", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Event resolved and payouts distributed!";
                    return RedirectToAction("Index");
                }
                else
                {
                     var error = await response.Content.ReadAsStringAsync();
                     ModelState.AddModelError("", "Failed to resolve event: " + error);
                      // Re-fetch event for display
                    try
                    {
                        var eventItem = await _httpClient.GetFromJsonAsync<Event>($"{ApiBaseUrl}/api/events/{EventId}");
                        return View(eventItem);
                    }
                    catch { return RedirectToAction("Index"); }
                }
            }
            catch
            {
                ModelState.AddModelError("", "Error communicating with server.");
                 // Re-fetch event for display
                try
                {
                    var eventItem = await _httpClient.GetFromJsonAsync<Event>($"{ApiBaseUrl}/api/events/{EventId}");
                    return View(eventItem);
                }
                catch { return RedirectToAction("Index"); }
            }
        }
    }
}
