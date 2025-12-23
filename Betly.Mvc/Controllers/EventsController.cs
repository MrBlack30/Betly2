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
                // Filter to show only unresolved events for betting
                var events = await _httpClient.GetFromJsonAsync<List<Event>>($"{ApiBaseUrl}/api/events");
                if (events != null)
                {
                    events = events.Where(e => !e.IsResolved).ToList();
                }
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

            // Set OwnerId from logged-in user
            var userIdStr = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userIdStr)) 
                return RedirectToAction("Login", "Account");
            
            model.OwnerId = int.Parse(userIdStr);

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
                
                // Get current user ID to pass as OwnerId for authorization check in API
                var userIdStr = User.FindFirst("UserId")?.Value;
                if (!string.IsNullOrEmpty(userIdStr))
                {
                    request.OwnerId = int.Parse(userIdStr);
                }

                var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/api/events/{EventId}/resolve", request);

                if (response.IsSuccessStatusCode)
                {
                    TempData["SuccessMessage"] = "Event resolved and payouts distributed!";
                    return RedirectToAction("MyEvents");
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
        [HttpGet]
        public async Task<IActionResult> MyEvents()
        {
            try
            {
                // This call relies on the cookie/token propagation or simpler authenticated HttpClient logic.
                // Since ApiBaseUrl is localhost, we might need to pass the UserId manually or ensure the API endpoint uses the claim.
                // However, the MVC controller uses `User.FindFirst("UserId")`.
                // The API controller ALSO uses User.FindFirst("UserId"). 
                // We need to make sure the HTTP request from MVC to API carries the auth context OR (simpler for now) pass user ID as query param if API allows, or stick to shared DB context usage in repo (but API is separate).
                // Wait, if API is separate process, MVC needs to send token.
                // Assuming we haven't implemented full JWT propagation, let's check how other calls work.
                // Ah, PlaceBet in MVC sends UserId in body. 
                // But MyEvents is GET.
                
                // CRITICAL FIX: The API `GetMyEvents` relies on `User` claims. 
                // The HttpClient in MVC is NOT sending the user token by default.
                // For this quick implementation, let's modify the API to accept UserId as query param for valid requests from MVC matching the logged in user.
                
                // actually, let's do this:
                var userIdStr = User.FindFirst("UserId")?.Value;
                if (string.IsNullOrEmpty(userIdStr)) return RedirectToAction("Login", "Account");
                
                // To keep it simple and consistent with "PlaceBet" which sends data:
                // We will call a modified API endpoint or just use the repo directly since we have direct DB access in data layer? 
                // No, we should stick to API. 
                // Let's modify the API call to pass the user ID as a header or query param for simplicity in this dev environment, 
                // OR since we are running locally, just fetch from API using a new endpoint that accepts ID: `api/events/owner/{id}`
                
                // Let's use the new endpoint `api/events/my-events` but we need to authenticate.
                // If auth is hard, let's just use `api/events` and filter client side? No, improper.
                
                // Let's add `api/events/owner/{ownerId}` to API for simplicity.
                var response = await _httpClient.GetFromJsonAsync<List<Event>>($"{ApiBaseUrl}/api/events/owner/{userIdStr}");
                return View(response);
            }
            catch
            {
                return View(new List<Event>());
            }
        }
    }
}
