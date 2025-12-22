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
            return await _context.Events.ToListAsync();
        }

        public async Task<Event> GetEventByIdAsync(int id)
        {
            return await _context.Events.FindAsync(id);
        }
    }
}
