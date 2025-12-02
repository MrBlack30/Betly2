
using Betly.core.Models;

namespace Betly.core.Interfaces
{
    // Contract defining the operations the API can perform on User data
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User> AddUserAsync(User user);
    }
}