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
        private readonly ILogger<AuthController> _logger;

        public AuthController(IUserService userService, ILogger<AuthController> logger)
        {
            _userService = userService;
            _logger = logger;
        }

        [HttpPost("register")]
        public async Task<IActionResult> Register(RegisterDTO dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
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
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user registration");
                return StatusCode(500, new { message = "Registration failed. Please try again." });
            }
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login(LoginDto dto)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var token = await _userService.LoginAsync(dto.Email, dto.Password);

                if (token == null)
                    return Unauthorized(new { message = "Invalid credentials" });

                return Ok(new { token });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during user login");
                return StatusCode(500, new { message = "Login failed. Please try again." });
            }
        }

        [Authorize]
        [HttpGet("me")]
        public IActionResult Me()
        {
            string? userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            string? usernameClaim = User.FindFirst(ClaimTypes.Name)?.Value;
            string? emailClaim = User.FindFirst(ClaimTypes.Email)?.Value;

            if (userIdClaim == null || !int.TryParse(userIdClaim, out int userId))
                return Unauthorized();

            return Ok(new
            {
                Id = userId,
                Username = usernameClaim,
                Email = emailClaim
            });
        }
    }
}