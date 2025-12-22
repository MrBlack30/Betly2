

namespace Betly.core.Models
{
    public class User
    {
        public int Id { get; set; }
        public string Email { get; set; } = string.Empty;

        // CRITICAL: NEVER store raw password. This will hold the HASHED password.
        public string PasswordHash { get; set; } = string.Empty;

        public decimal Balance { get; set; } = 0.00m;

        // Navigation properties (for EF Core relationships)
        public ICollection<Bet> Bets { get; set; } = new List<Bet>();
    }
}