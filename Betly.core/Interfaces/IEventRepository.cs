using System.Collections.Generic;
using System.Threading.Tasks;
using Betly.core.Models;

namespace Betly.core.Interfaces
{
    public interface IEventRepository
    {
        Task<List<Event>> GetAllEventsAsync();
        Task<Event> GetEventByIdAsync(int id);
        Task<Event> AddEventAsync(Event eventItem);
    }
}
