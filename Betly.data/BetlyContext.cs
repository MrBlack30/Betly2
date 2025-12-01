using Betly.core; // Assuming Betly.Core contains your models
using Microsoft.EntityFrameworkCore;

namespace Betly.Data
{
    public class BetlyContext : DbContext
    {
        // 1. Constructor: This is where the configuration (like the connection string) 
        //    will be passed from the Betly.Api project's Program.cs.
        public BetlyContext(DbContextOptions<BetlyContext> options)
            : base(options)
        {
        }

        // 2. DbSet Properties: These represent the tables in your Azure SQL Database.
        public DbSet<User> Users { get; set; }
       // public DbSet<Event> Events { get; set; }
        public DbSet<Bet> Bets { get; set; }
       // public DbSet<Result> Results { get; set; }
        //public DbSet<Transaction> Transactions { get; set; }

        // 3. (Optional but Recommended) Model Configuration: 
        //    Used for more advanced configurations like composite keys,
        //    specifying column types, or setting up complex relationships.
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Example of configuring a relationship (to ensure correct foreign keys)
            // modelBuilder.Entity<Bet>()
            //    .HasOne(b => b.User)
            //    .WithMany(u => u.Bets)
            //    .HasForeignKey(b => b.UserId);

            // Call the base implementation
            base.OnModelCreating(modelBuilder);
        }
    }
}