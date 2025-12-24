using System.Collections.Generic;
using System.Threading.Tasks;
using Betly.core.Models;

namespace Betly.core.Interfaces
{
    public interface IFriendRepository
    {
        Task<Friendship?> GetFriendshipAsync(int userAId, int userBId);
        Task<List<Friendship>> GetFriendshipsByUserIdAsync(int userId);
        Task<List<Friendship>> GetPendingRequestsByUserIdAsync(int userId);
        Task AddFriendshipAsync(Friendship friendship);
        Task UpdateFriendshipAsync(Friendship friendship);
    }
}
