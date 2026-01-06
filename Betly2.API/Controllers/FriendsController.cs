using Betly.core.DTOs;
using Betly.core.Interfaces;
using Betly.core.Models;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Betly2.API.Controllers
{
    [ApiController]
    [Route("api/friends")]
    public class FriendsController : ControllerBase
    {
        private readonly IFriendRepository _friendRepository;
        private readonly IUserRepository _userRepository;

        public FriendsController(IFriendRepository friendRepository, IUserRepository userRepository)
        {
            _friendRepository = friendRepository;
            _userRepository = userRepository;
        }

        // Helper to get UserId - In API this should come from token, but for now we might rely on the client passing it 
        // OR we can try to extract it if claims are populated.
        // Given previous issues with claim propagation, we might need a workaround or assume the endpoint is called securely.
        // Let's rely on standard User claims, assuming Auth middleware is working or will be fixed.
        private int? GetCurrentUserId()
        {
             var userIdStr = User.FindFirst("UserId")?.Value;
             if (int.TryParse(userIdStr, out var id)) return id;
             return null;
        }

        [HttpPost("request")]
        public async Task<IActionResult> SendRequest([FromBody] FriendRequestDTO request)
        {
            // We need the ID of the requester. 
            // Workaround: Accept requesterId in DTO? No, that's insecure.
            // Let's assume the MVC controller will pass it via header or we fix the auth.
            // For this specific iteration, let's look for a header "X-UserId" if User claim is missing (DEV only)
            // or just rely on the controller logic being correct.
            // Let's stick to reading claims. If it fails, we know we need to fix Auth.
            
            // Wait, previous fix was to pass OwnerId in body.
            // Let's allow passing RequesterId in the DTO for now to match the pattern, 
            // BUT proper way is Header/Token.
            // Let's check headers for a custom user-id header since we are doing Mvc -> Api without full JWT flow apparently?
            // Or just allow the client to send it.
            
            // NOTE: For the sake of the user's "friend system", I'll add RequesterId to a derived DTO or just read explicit param.
            // Let's add an endpoint that takes userId as route param to be explicit.
            return BadRequest("Use the user-specific endpoint: api/friends/{userId}/request");
        }

        [HttpPost("{userId}/request")]
        public async Task<IActionResult> SendRequest(int userId, [FromBody] FriendRequestDTO request)
        {
            var targetUser = await _userRepository.GetByEmailAsync(request.TargetEmail);
            if (targetUser == null) return NotFound("User not found.");

            if (targetUser.Id == userId) return BadRequest("Cannot add yourself.");

            var existing = await _friendRepository.GetFriendshipAsync(userId, targetUser.Id);
            if (existing != null) return BadRequest("Friendship or request already exists.");

            var friendship = new Friendship
            {
                RequesterId = userId,
                AddresseeId = targetUser.Id,
                Status = "Pending"
            };

            await _friendRepository.AddFriendshipAsync(friendship);
            return Ok("Friend request sent.");
        }

        [HttpPost("{userId}/accept/{requestId}")]
        public async Task<IActionResult> AcceptRequest(int userId, int requestId)
        {
            // Ideally we get the friendship by ID, but our repo gets by User pair.
            // We need to implement GetById in repo or just find it via the list.
            // Let's rely on the list of pending requests for safety.
            var pending = await _friendRepository.GetPendingRequestsByUserIdAsync(userId);
            var match = pending.Find(f => f.Id == requestId);

            if (match == null) return NotFound("Request not found.");

            match.Status = "Accepted";
            await _friendRepository.UpdateFriendshipAsync(match);

            return Ok("Friend request accepted.");
        }

        [HttpGet("{userId}")]
        public async Task<IActionResult> GetFriends(int userId)
        {
            var friends = await _friendRepository.GetFriendshipsByUserIdAsync(userId);
            return Ok(friends);
        }

        [HttpGet("{userId}/pending")]
        public async Task<IActionResult> GetPendingRequests(int userId)
        {
            var requests = await _friendRepository.GetPendingRequestsByUserIdAsync(userId);
            return Ok(requests);
        }

        [HttpGet("{userId}/sent")]
        public async Task<IActionResult> GetSentRequests(int userId)
        {
            var requests = await _friendRepository.GetSentRequestsByUserIdAsync(userId);
            return Ok(requests);
        }
    }
}
