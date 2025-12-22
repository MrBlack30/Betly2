using Betly.core.DTOs;
using Betly.core.Models;

namespace Betly.Mvc.Models
{
    public class DashboardViewModel
    {
        public UserDto User { get; set; } = new UserDto();
        public List<Bet> Bets { get; set; } = new List<Bet>();
    }
}
