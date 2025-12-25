using AuthService.DTOs;
using AuthService.Models;
using AuthService.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace AuthService.Controllers
{
    [ApiController]
    [Route("api/[controller]")] //controller is a placeholder, in this case it will be auth
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            User user = await _userService.CreateUserAsync(dto.Username, dto.Email, dto.Password);

            return Ok(new
            {
                user.Id,
                user.Username,
                user.Email,
                user.CreatedAt
            });
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            var token = await _userService.LoginAsync(dto.Email, dto.Password);

            if (token == null)
            {
                return Unauthorized(new { message = "Invalid credentials" });
            };

            return Ok(new { token });
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            string? emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userIdClaim == null) return Unauthorized();

            return Ok(new
            {
                Id = userIdClaim,
                Username = usernameClaim,
                Email = emailClaim
            });
        }
    }
}