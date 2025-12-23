using System.ComponentModel.DataAnnotations;

namespace Betly.core.DTOs
{
    public class ResolveEventRequest
    {
        [Required]
        public string WinningOutcome { get; set; } = string.Empty;
        public int OwnerId { get; set; }
    }
}
