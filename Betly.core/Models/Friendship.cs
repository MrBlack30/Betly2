using System;

namespace Betly.core.Models
{
    public class Friendship
    {
        public int Id { get; set; }
        public int RequesterId { get; set; }
        public int AddresseeId { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Declined
        public DateTime CreatedAt { get; set; } = DateTime.Now;

        // Navigation properties
        public User? Requester { get; set; }
        public User? Addressee { get; set; }
    }
}
