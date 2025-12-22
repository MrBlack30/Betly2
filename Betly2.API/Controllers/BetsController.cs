using Betly.core.DTOs;
using Betly.core.Interfaces;
using Betly.core.Models;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Betly2.API.Controllers
{
    [ApiController]
    [Route("api/bets")]
    public class BetsController : ControllerBase
    {
        private readonly IBetRepository _betRepository;
        private readonly IUserRepository _userRepository;
        private readonly IEventRepository _eventRepository;

        public BetsController(IBetRepository betRepository, IUserRepository userRepository, IEventRepository eventRepository)
        {
            _betRepository = betRepository;
            _userRepository = userRepository;
            _eventRepository = eventRepository;
        }

        [HttpPost]
        public async Task<IActionResult> PlaceBet([FromBody] BetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var bet = new Bet
                {
                    UserId = request.UserId,
                    EventId = request.EventId,
                    Amount = request.Amount,
                    SelectedOutcome = request.SelectedOutcome,
                    Outcome = "Pending"
                };

                await _betRepository.PlaceBetAsync(bet);
                
                return Ok(new { message = "Bet placed successfully.", betId = bet.Id });
            }
            catch (System.Exception ex)
            {
                if (ex.Message == "Insufficient funds")
                    return BadRequest(new { message = "Insufficient funds." });
                if (ex.Message == "User not found")
                    return NotFound(new { message = "User not found." });
                    
                return StatusCode(500, new { message = "An error occurred while placing the bet." });
            }
        }
    }
}
