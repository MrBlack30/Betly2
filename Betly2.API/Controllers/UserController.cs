using Microsoft.AspNetCore.Mvc;
using Betly.core.DTOs;
using Betly.core.Interfaces;
using Betly.core.Models;
using Betly.Api.Utilities; // For PasswordHasher

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;

    // Dependency Injection: Framework provides the UserRepository
    public UserController(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    [HttpPost("register")] // Route: POST /api/users/register
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest model)
    {
        // 1. Check if user already exists
        if (await _userRepository.GetByEmailAsync(model.Email) != null)
        {
            return BadRequest(new { message = "Email is already registered." });
        }

        // 2. Hash Password and create User entity
        var newUser = new User
        {
            Email = model.Email,
            PasswordHash = PasswordHasher.HashPassword(model.Password),
            Balance = 0.00m // Default starting balance
        };

        // 3. Save to database
        await _userRepository.AddUserAsync(newUser);

        // 4. Return success (exclude the password hash from the response)
        return Ok(new { message = "Registration successful." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var user = await _userRepository.GetByEmailAsync(model.Email);
        if (user == null)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // Verify Password
        var passwordHash = PasswordHasher.HashPassword(model.Password);
        if (user.PasswordHash != passwordHash)
        {
            return Unauthorized(new { message = "Invalid email or password." });
        }

        // Return the user details (excluding password)

        return Ok(new
        {
            message = "Login successful",
            user = new { user.Id, user.Email, user.Balance }
        });
    }

    [HttpGet("{email}/bets")]
    public async Task<IActionResult> GetBets(string email)
    {
        var bets = await _userRepository.GetBetsByEmailAsync(email);
        return Ok(bets);
    }
}