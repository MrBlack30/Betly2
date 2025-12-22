using Microsoft.AspNetCore.Mvc;
using Betly.core.DTOs;
using Betly.core.Interfaces;
using Betly.core.Models;
using Betly.Api.Utilities;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;
using System.Text;

[ApiController]
[Route("api/users")]
public class UserController : ControllerBase
{
    private readonly IUserRepository _userRepository;
    private readonly IConfiguration _configuration;

    public UserController(IUserRepository userRepository, IConfiguration configuration)
    {
        _userRepository = userRepository;
        _configuration = configuration;
    }

    [HttpPost("register")]
    public async Task<IActionResult> RegisterUser([FromBody] RegisterRequest model)
    {
        if (await _userRepository.GetByEmailAsync(model.Email) != null)
        {
            return BadRequest(new { message = "Email is already registered." });
        }

        var newUser = new User
        {
            Email = model.Email,
            PasswordHash = PasswordHasher.HashPassword(model.Password),
            Balance = 0.00m
        };

        await _userRepository.AddUserAsync(newUser);
        return Ok(new { message = "Registration successful." });
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest model)
    {
        var user = await _userRepository.GetByEmailAsync(model.Email);
        if (user == null) return Unauthorized(new { message = "Invalid email or password." });

        var passwordHash = PasswordHasher.HashPassword(model.Password);
        if (user.PasswordHash != passwordHash) return Unauthorized(new { message = "Invalid email or password." });

        // יצירת הטוקן
        var token = GenerateJwtToken(user);

        // Return the user details (excluding password)
        return Ok(new 
        { 
            message = "Login successful", 
            token = token,
            user = new { user.Id, user.Email, user.Balance } 
        });
    }
}