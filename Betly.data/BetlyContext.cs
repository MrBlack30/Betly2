using Microsoft.EntityFrameworkCore;
using Betly.core.Models;

namespace Betly.data
{
    public class BetlyContext : DbContext
    {
        public DbSet<Bet> Bets { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Event> Events { get; set; }

        public BetlyContext(DbContextOptions<BetlyContext> options) : base(options) { }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Configure Bet.Amount - 18 digits total, 2 decimal places (e.g., $999,999,999,999,999.99)
            modelBuilder.Entity<Bet>()
                .Property(b => b.Amount)
                .HasPrecision(18, 2);

            // Configure User.Balance - 18 digits total, 2 decimal places
            modelBuilder.Entity<User>()
                .Property(u => u.Balance)
                .HasPrecision(18, 2);
        }
    }
}