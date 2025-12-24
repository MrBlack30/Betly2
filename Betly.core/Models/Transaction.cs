using System;

namespace Betly.core.Models
{
    public class Transaction
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string Type { get; set; } = string.Empty; // "Deposit", "Bet", "Win"
        public decimal Amount { get; set; }
        public decimal BalanceAfter { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;

        // Navigation property
        // public User User { get; set; } // Optional, can enable if needed for EF
    }
}
