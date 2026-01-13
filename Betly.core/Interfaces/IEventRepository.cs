using System.Collections.Generic;
using System.Threading.Tasks;
using Betly.core.Models;

namespace Betly.core.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<List<Event>> GetVisibleEventsAsync(int currentUserId, List<int> friendIds);
        Task<Event> GetEventByIdAsync(int id);
        Task<Event> AddEventAsync(Event eventItem, List<int> invitedUserIds);
        Task UpdateEventAsync(Event eventItem);
        Task<Event?> GetEventWithBetsByIdAsync(int id);
        Task<List<Event>> GetEventsByOwnerAsync(int ownerId);
    }
}
