using System.Collections.Generic;
using Betly.core.Interfaces;
using Betly.core.Models;
using Microsoft.EntityFrameworkCore;
using Betly.data;

namespace Betly.data.Repositories
{
    // Concrete implementation of the IUserRepository interface
    public class UserRepository : IUserRepository
    {
        private readonly BetlyContext _context;

        public UserRepository(BetlyContext context)
        {
            _context = context;
        }

        public async Task<User> GetByEmailAsync(string email)
        {
            return await _context.Users.FirstOrDefaultAsync(u => u.Email == email);
        }

        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.Users.FindAsync(id);
        }

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }

        public async Task UpdateUserAsync(User user)
        {
            _context.Users.Update(user);
            await _context.SaveChangesAsync();
        }

        public async Task<List<Bet>> GetBetsByEmailAsync(string email)
        {
            return await _context.Users
                .Where(u => u.Email == email)
                .SelectMany(u => u.Bets)
                .Include(b => b.Event)
                .ToListAsync();
        }

        public async Task AddTransactionAsync(Transaction transaction)
        {
            _context.Transactions.Add(transaction);
            await _context.SaveChangesAsync();
        }
    }
}