using Betly.core.Models;
using System.Collections.Generic;

namespace Betly.core.DTOs
{
    public class CreateEventRequest
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string TeamA { get; set; } = string.Empty;
        public string TeamB { get; set; } = string.Empty;
        public decimal OddsTeamA { get; set; }
        public decimal OddsTeamB { get; set; }
        public decimal OddsDraw { get; set; }
        public int OwnerId { get; set; }
        public bool IsPublic { get; set; } = true;
        public List<int> InvitedUserIds { get; set; } = new List<int>();
    }
}
