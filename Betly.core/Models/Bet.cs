using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace Betly.core.Models
{
    public class Bet
    {
        public int Id { get; set; } // Primary Key (PK)
        
        [Column(TypeName = "decimal(18, 2)")]
        public decimal Amount { get; set; } // Column

        public int UserId { get; set; }
        public User? User { get; set; }

        public int EventId { get; set; }
        public Event? Event { get; set; }

        [Required]
        public string SelectedOutcome { get; set; } = string.Empty; // e.g. "TeamA", "TeamB", "Draw"
        
        public string Outcome { get; set; } = "Pending"; // "Pending", "Won", "Lost"
    }
}
