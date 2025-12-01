namespace Betly.core
{
    public class User
    {
        public int Id { get; set; } // Primary Key (PK)
        public string Username { get; set; } // Column
        public string Email { get; set; }
        public decimal Balance { get; set; }
        // Navigation property for 1-to-many relationship
        public ICollection<Bet> Bets { get; set; }
    }
}
