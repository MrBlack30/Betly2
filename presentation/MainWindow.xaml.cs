using Betly.core.DTOs; // Important: Reference the DTO
using System;
using System.Collections.Generic;
using System.Net.Http;

using System.Windows;

namespace Betly.Wpf
{
    public partial class MainWindow : Window
    {
        // 💡 CRITICAL: Ensure this URL matches your Betly.Api project's running address (e.g., https://localhost:7001)
        private const string ApiBaseUrl = "https://localhost:7001";
        private readonly HttpClient _httpClient = new HttpClient();

        public MainWindow()
        {
            InitializeComponent();
        }

        private async void RegisterButton_Click(object sender, RoutedEventArgs e)
        {
            // Clear previous status messages
            StatusText.Text = string.Empty;
            StatusText.Foreground = System.Windows.Media.Brushes.Red;

            // 1. Collect Input
            var request = new RegisterRequest
            {
                Email = EmailInput.Text,
                Password = PasswordInput.Password // Get text from PasswordBox
            };

            // Basic client-side validation (optional, but good practice)
            if (string.IsNullOrWhiteSpace(request.Email) || string.IsNullOrWhiteSpace(request.Password))
            {
                StatusText.Text = "Email and Password are required.";
                return;
            }

            // 2. Call the API Endpoint
            try
            {
                // The API endpoint we created earlier: POST /api/users/register
                string url = $"{ApiBaseUrl}/api/users/register";

                // Send the JSON payload
                var response = await _httpClient.PostAsJsonAsync(url, request);

                if (response.IsSuccessStatusCode)
                {
                    // Success (HTTP 200/201)
                    StatusText.Text = "Registration successful! User created in SQL DB.";
                    StatusText.Foreground = System.Windows.Media.Brushes.Green;
                    EmailInput.Text = string.Empty;
                    PasswordInput.Password = string.Empty;
                }
                else
                {
                    // Failure (e.g., HTTP 400 Bad Request, "Email already registered.")
                    var errorContent = await response.Content.ReadFromJsonAsync<Dictionary<string, string>>();
                    string errorMessage = errorContent != null && errorContent.ContainsKey("message")
                        ? errorContent["message"]
                        : $"Registration failed: Status code {response.StatusCode}.";

                    StatusText.Text = errorMessage;
                }
            }
            catch (HttpRequestException ex)
            {
                StatusText.Text = $"Connection Error. Is the Betly.Api project running? ({ex.Message})";
            }
            catch (Exception ex)
            {
                StatusText.Text = $"An unexpected error occurred: {ex.Message}";
            }
        }
    }
}