using System.ComponentModel.DataAnnotations;

namespace Betly.core.DTOs
{
    public class BetRequest
    {
        [Required]
        public int UserId { get; set; }
        
        [Required]
        public int EventId { get; set; }
        
        [Required]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than zero")]
        public decimal Amount { get; set; }
        
        [Required]
        public string SelectedOutcome { get; set; } = string.Empty; // "TeamA", "TeamB", "Draw"
    }
}
