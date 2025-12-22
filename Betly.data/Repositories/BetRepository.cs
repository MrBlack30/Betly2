using Betly.core.Interfaces;
using Betly.core.Models;
using System.Threading.Tasks;

namespace Betly.data.Repositories
{
    public class BetRepository : IBetRepository
    {
        private readonly BetlyContext _context;

        public BetRepository(BetlyContext context)
        {
            _context = context;
        }

        public async Task<Bet> AddBetAsync(Bet bet)
        {
            _context.Bets.Add(bet);
            await _context.SaveChangesAsync();
            return bet;
        }

        public async Task<Bet> PlaceBetAsync(Bet bet)
        {
            // Use execution strategy for resilience if needed, but simple transaction here
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                var user = await _context.Users.FindAsync(bet.UserId);
                if (user == null) 
                    throw new System.Exception("User not found");
                
                if (user.Balance < bet.Amount)
                    throw new System.Exception("Insufficient funds");

                // Deduct balance
                user.Balance -= bet.Amount;
                
                // Add bet
                _context.Bets.Add(bet);
                
                await _context.SaveChangesAsync();
                await transaction.CommitAsync();
                
                return bet;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
    }
}
