using Betly.core.Models;
using Betly.data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Betly2.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TransactionsController : ControllerBase
    {
        private readonly BetlyContext _context;

        public TransactionsController(BetlyContext context)
        {
            _context = context;
        }

        [HttpGet("{userId}")]
        public async Task<ActionResult<List<Transaction>>> GetTransactions(
            int userId, 
            [FromQuery] string? type = null, 
            [FromQuery] DateTime? startDate = null, 
            [FromQuery] DateTime? endDate = null)
        {
            var query = _context.Transactions.Where(t => t.UserId == userId);

            if (!string.IsNullOrEmpty(type))
            {
                query = query.Where(t => t.Type == type);
            }

            if (startDate.HasValue)
            {
                query = query.Where(t => t.Timestamp >= startDate.Value);
            }

            if (endDate.HasValue)
            {
                // Ensure we include the whole end day if time is not specified
                var effectiveEnd = endDate.Value.Date.AddDays(1).AddTicks(-1);
                query = query.Where(t => t.Timestamp <= effectiveEnd);
            }

            var transactions = await query
                .OrderByDescending(t => t.Timestamp)
                .ToListAsync();

            return transactions;
        }
    }
}
