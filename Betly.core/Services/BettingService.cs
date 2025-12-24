using System;
using System.Linq;
using System.Threading.Tasks;
using Betly.core.Interfaces;
using Betly.core.Models;

namespace Betly.core.Services
{
    public class BettingService
    {
        private readonly IEventRepository _eventRepository;
        private readonly IUserRepository _userRepository;

        public BettingService(IEventRepository eventRepository, IUserRepository userRepository)
        {
            _eventRepository = eventRepository;
            _userRepository = userRepository;
        }

        public async Task ResolveEventAsync(int eventId, string winningOutcome)
        {
            var eventItem = await _eventRepository.GetEventWithBetsByIdAsync(eventId);
            if (eventItem == null)
            {
                throw new Exception("Event not found");
            }

            if (eventItem.IsResolved)
            {
                throw new Exception("Event is already resolved");
            }

            eventItem.IsResolved = true;
            eventItem.Winner = winningOutcome;

            var allBets = eventItem.Bets;
            
            // Normalize strings for comparison
            string normalizedWinningOutcome = winningOutcome.Trim();

            var winningBets = allBets.Where(b => b.SelectedOutcome.Trim().Equals(normalizedWinningOutcome, StringComparison.OrdinalIgnoreCase)).ToList();
            var losingBets = allBets.Where(b => !b.SelectedOutcome.Trim().Equals(normalizedWinningOutcome, StringComparison.OrdinalIgnoreCase)).ToList();

            decimal totalLoserPool = losingBets.Sum(b => b.Amount);
            decimal totalWinningBetAmount = winningBets.Sum(b => b.Amount);

            // Distribute winnings
            if (winningBets.Any())
            {
                foreach (var winner in winningBets)
                {
                    // Calculate share: (MyBet / TotalWinnerBets) * TotalLoserPool
                    decimal shareRatio = winner.Amount / totalWinningBetAmount;
                    decimal winnings = winner.Amount + (shareRatio * totalLoserPool);

                    // Update User Balance
                    int winnerUserId = 0;
                    decimal currentBalance = 0;

                    // Update User Balance
                    if (winner.User != null)
                    {
                        winner.User.Balance += winnings;
                        winnerUserId = winner.User.Id;
                        currentBalance = winner.User.Balance;
                    }
                    else
                    {
                        // In case User wasn't loaded
                        var user = await _userRepository.GetByIdAsync(winner.UserId);
                        if (user != null)
                        {
                            user.Balance += winnings;
                            // Explicitly update user if it was fetched separately
                            await _userRepository.UpdateUserAsync(user);
                            winnerUserId = user.Id;
                            currentBalance = user.Balance;
                        }
                    }

                    if (winnerUserId > 0)
                    {
                        await _userRepository.AddTransactionAsync(new Transaction
                        {
                            UserId = winnerUserId,
                            Type = "Win",
                            Amount = winnings,
                            BalanceAfter = currentBalance,
                            Timestamp = DateTime.UtcNow
                        });
                    }

                    winner.Outcome = "Won";
                }
            }
            else
            {
                // Edge case: No winners. House takes all? Or refund?
                // For now, let's keep it simple: Losers lose, money disappears (or goes to house/admin if implemented).
                // Or maybe refund? The prompt says "split money of all losers", logic implies if no winners, no one splits.
            }

            // Mark losers
            foreach (var loser in losingBets)
            {
                loser.Outcome = "Lost";
            }

            await _eventRepository.UpdateEventAsync(eventItem);
        }
    }
}
