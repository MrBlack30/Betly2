using Betly.core.Interfaces;
using Betly.core.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Betly.data.Repositories
{
    public class EventRepository : IEventRepository
    {
        private readonly BetlyContext _context;

        public EventRepository(BetlyContext context)
        {
            _context = context;
        }

        public async Task<List<Event>> GetAllEventsAsync()
        {
            return await _context.Events.Include(e => e.Owner).ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _context.Events.Include(e => e.Owner).FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<Event> AddEventAsync(Event eventItem)
        {
            _context.Events.Add(eventItem);
            await _context.SaveChangesAsync();
            return eventItem;
        }

        public async Task UpdateEventAsync(Event eventItem)
        {
            _context.Events.Update(eventItem);
            await _context.SaveChangesAsync();
        }

        public async Task<Event?> GetEventWithBetsByIdAsync(int id)
        {
            return await _context.Events
                .Include(e => e.Bets)
                .ThenInclude(b => b.User)
                .FirstOrDefaultAsync(e => e.Id == id);
        }

        public async Task<List<Event>> GetVisibleEventsAsync(int currentUserId, List<int> friendIds)
        {
            return await _context.Events
                .Include(e => e.Owner)
                .Where(e => e.IsPublic || e.OwnerId == currentUserId || friendIds.Contains(e.OwnerId))
                .ToListAsync();
        }

        public async Task<List<Event>> GetEventsByOwnerAsync(int ownerId)
        {
            return await _context.Events
                .Include(e => e.Owner)
                .Where(e => e.OwnerId == ownerId)
                .ToListAsync();
        }
    }
}
