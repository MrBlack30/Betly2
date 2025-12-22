using System;
using System.Collections.Generic;

namespace Betly.core.Models
{
    public class Event
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public DateTime Date { get; set; }
        public string TeamA { get; set; } = string.Empty;
        public string TeamB { get; set; } = string.Empty;
        public decimal OddsTeamA { get; set; }
        public decimal OddsTeamB { get; set; }
        public decimal OddsDraw { get; set; }

        // Navigation property
        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}
