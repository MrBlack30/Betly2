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
}