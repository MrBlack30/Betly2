using System.ComponentModel.DataAnnotations;

namespace Betly.core.DTOs
{
    // Data Transfer Object: Defines the data structure received from the client
    public class RegisterRequest
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; } = string.Empty;

        [Required]
        [MinLength(7, ErrorMessage = "Password must be at least 7 characters.")]
        public string Password { get; set; } = string.Empty;
    }
}