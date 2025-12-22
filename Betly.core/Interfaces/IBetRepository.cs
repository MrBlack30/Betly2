using System.Threading.Tasks;
using Betly.core.Models;

namespace Betly.core.Interfaces
{
    public interface IBetRepository
    {
        Task<Bet> AddBetAsync(Bet bet);
        Task<Bet> PlaceBetAsync(Bet bet);
    }
}
