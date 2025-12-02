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

        public async Task<User> AddUserAsync(User user)
        {
            _context.Users.Add(user);
            await _context.SaveChangesAsync();
            return user;
        }
    }
}