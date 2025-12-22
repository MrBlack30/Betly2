using Microsoft.AspNetCore.Mvc;
using Betly.core.DTOs; // Use the DTO from your Core project
using Betly.core.Models;
using System.Net.Http.Json; // For PostAsJsonAsync

namespace Betly.Mvc.Controllers
{
    public class AccountController : Controller
    {
        // CRITICAL: Ensure this URL matches your Betly.Api project's running address
        private const string ApiBaseUrl = "https://localhost:7203";
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

    // Login Action (GET /Account/Login)
    [HttpGet]
    public IActionResult Login()
    {
        if (TempData["SuccessMessage"] != null)
        {
            ViewBag.Message = TempData["SuccessMessage"];
        }
        return View();
    }

    // Login Action (POST /Account/Login)
    [HttpPost]
    public async Task<IActionResult> Login(LoginRequest model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            string url = $"{ApiBaseUrl}/api/users/login";
            var response = await _httpClient.PostAsJsonAsync(url, model);

            if (response.IsSuccessStatusCode)
            {
                // Create user identity
                var claims = new List<System.Security.Claims.Claim>
                {
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Name, model.Email),
                    new System.Security.Claims.Claim(System.Security.Claims.ClaimTypes.Email, model.Email),
                    // Assuming API returns user object with Id. We need to parse it from response.
                    // The standard PostAsJsonAsync response reading is tricky if purely dynamic.
                    // Need to deserialize response correctly first. 
                };
                
                // Read response content to get User ID
                var loginResponse = await response.Content.ReadFromJsonAsync<LoginResponse>();
                 if (loginResponse?.User != null)
                {
                     claims.Add(new System.Security.Claims.Claim("UserId", loginResponse.User.Id.ToString()));
                }

                var claimsIdentity = new System.Security.Claims.ClaimsIdentity(claims, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
                var authProperties = new Microsoft.AspNetCore.Authentication.AuthenticationProperties
                {
                    IsPersistent = true
                };

                await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignInAsync(
                    HttpContext,
                    Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme, 
                    new System.Security.Claims.ClaimsPrincipal(claimsIdentity), 
                    authProperties);

                return RedirectToAction("Dashboard", "Account");
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
                return View(model);
            }
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "An error occurred while communicating with the API.");
            return View(model);
        }
    }

    [HttpGet]
    public async Task<IActionResult> Dashboard()
    {
        var email = User.Identity?.Name;
        if (string.IsNullOrEmpty(email))
        {
            return RedirectToAction("Login");
        }

        try
        {
            var userId = User.FindFirst("UserId")?.Value;
            if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");
            
            // Fetch User Details (Balance)
            var userResponse = await _httpClient.GetFromJsonAsync<UserDto>($"{ApiBaseUrl}/api/users/{userId}");
            
            // Fetch Bets
            var betsResponse = await _httpClient.GetFromJsonAsync<List<Bet>>($"{ApiBaseUrl}/api/users/{email}/bets");

            var viewModel = new Betly.Mvc.Models.DashboardViewModel
            {
                User = userResponse ?? new UserDto(),
                Bets = betsResponse ?? new List<Bet>()
            };

            return View(viewModel);
        }
        catch (Exception)
        {
            ModelState.AddModelError(string.Empty, "Could not fetch dashboard data.");
            return View(new Betly.Mvc.Models.DashboardViewModel());
        }
    }

    [HttpGet]
    public IActionResult AddCredit()
    {
        return View();
    }

    [HttpPost]
    public async Task<IActionResult> AddCredit(AddCreditRequest model)
    {
        if (model.Amount <= 0)
        {
            ModelState.AddModelError("Amount", "Amount must be positive.");
            return View(model);
        }

        var userId = User.FindFirst("UserId")?.Value;
        if (string.IsNullOrEmpty(userId)) return RedirectToAction("Login");

        try
        {
            var response = await _httpClient.PostAsJsonAsync($"{ApiBaseUrl}/api/users/{userId}/credit", model);
            if (response.IsSuccessStatusCode)
            {
                TempData["SuccessMessage"] = "Credit added successfully!";
                return RedirectToAction("Dashboard");
            }
            else
            {
                ModelState.AddModelError("", "Failed to add credit.");
                return View(model);
            }
        }
        catch
        {
            ModelState.AddModelError("", "Service unavailable.");
            return View(model);
        }
    }

    public async Task<IActionResult> Logout()
    {
        await Microsoft.AspNetCore.Authentication.AuthenticationHttpContextExtensions.SignOutAsync(HttpContext, Microsoft.AspNetCore.Authentication.Cookies.CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Login");
    }
}
}