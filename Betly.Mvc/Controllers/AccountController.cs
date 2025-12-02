using Microsoft.AspNetCore.Mvc;
using Betly.core.DTOs; // Use the DTO from your Core project
using System.Net.Http.Json; // For PostAsJsonAsync

namespace Betly.Mvc.Controllers
{
    public class AccountController : Controller
    {
        // CRITICAL: Ensure this URL matches your Betly.Api project's running address
        private const string ApiBaseUrl = "https://localhost:7001";
        private readonly HttpClient _httpClient;

        // Dependency Injection for HttpClient is preferred, but simple init works too
        public AccountController()
        {
            _httpClient = new HttpClient();
        }

        // Action for displaying the registration form (GET /Account/Register)
        public IActionResult Register()
        {
            // The view engine will look for Views/Account/Register.cshtml
            return View();
        }

        // Action for handling the form submission (POST /Account/Register)
        [HttpPost]
        public async Task<IActionResult> Register(RegisterRequest model)
        {
            // 1. Basic validation (optional, as the API validates too)
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 2. Call the Betly.Api endpoint
            try
            {
                string url = $"{ApiBaseUrl}/api/users/register";
                
                // Post the RegisterRequest model to your API
                var response = await _httpClient.PostAsJsonAsync(url, model);

                if (response.IsSuccessStatusCode)
                {
                    // Registration successful
                    // Redirect the user to a success page or login page
                    TempData["SuccessMessage"] = "Registration successful! You can now log in.";
                    return RedirectToAction("Login", "Account");
                }
                else
                {
                    // Handle API errors (e.g., email already registered)
                    var errorContent = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    string errorMessage = errorContent != null && errorContent.ContainsKey("message") 
                        ? errorContent["message"] 
                        : "Registration failed. Please check your details.";
                    
                    ModelState.AddModelError(string.Empty, errorMessage);
                    return View(model);
                }
            }
            catch (Exception)
            {
                ModelState.AddModelError(string.Empty, "An unexpected error occurred. Please try again later.");
                return View(model);
            }
        }

        // Dummy Login Action (for redirect target)
        public IActionResult Login()
        {
            if (TempData["SuccessMessage"] != null)
            {
                ViewBag.Message = TempData["SuccessMessage"];
            }
            return View();
        }
    }
}