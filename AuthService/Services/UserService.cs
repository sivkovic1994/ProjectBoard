using AuthService.Data;
using AuthService.Models;
using Microsoft.AspNetCore.Cryptography.KeyDerivation;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;

namespace AuthService.Services
{
    public class UserService : IUserService
    {
        private readonly AuthDbContext _dbContext;
        private readonly IConfiguration _configuration;

        public UserService(AuthDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _configuration = configuration;
        }

        #region Public Methods

        public async Task<User> CreateUserAsync(string username, string email, string password)
        {
            username = username?.Trim() ?? string.Empty;
            email = email?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(username))
                throw new ArgumentException("Username cannot be empty");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email cannot be empty");

            if (await _dbContext.Users.AnyAsync(u => u.Email == email))
                throw new InvalidOperationException("Email already exists");

            if (await _dbContext.Users.AnyAsync(u => u.Username == username))
                throw new InvalidOperationException("Username already exists");

            User user = new User()
            {
                Username = username,
                Email = email,
                PasswordHash = HashPassword(password)
            };

            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();

            return user;
        }

        public async Task<string?> LoginAsync(string email, string password)
        {
            email = email?.Trim() ?? string.Empty;

            if (string.IsNullOrWhiteSpace(email))
                return null;

            User? user = await _dbContext.Users.FirstOrDefaultAsync(u => u.Email == email);

            if (user == null) return null;

            if (!VerifyPassword(password, user.PasswordHash)) return null;

            return GenerateJwt(user);
        }

        #endregion

        #region Helpers

        private bool VerifyPassword(string enteredPassword, string storedHash)
        {
            var parts = storedHash.Split('.');
            if (parts.Length != 2) return false;

            var salt = Convert.FromBase64String(parts[0]);
            var hash = parts[1];

            string enteredHash = Convert.ToBase64String(KeyDerivation.Pbkdf2(
                password: enteredPassword,
                salt: salt,
                prf: KeyDerivationPrf.HMACSHA256,
                iterationCount: 10000,
                numBytesRequested: 32
            ));

            return hash == enteredHash;
        }

        private string HashPassword(string password)
        {
            byte[] salt = new byte[16];

            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            string hashed = Convert.ToBase64String(KeyDerivation.Pbkdf2(password, salt, KeyDerivationPrf.HMACSHA256, 10000, 32));

            return $"{Convert.ToBase64String(salt)}.{hashed}";
        }

        private string GenerateJwt(User user)
        {
            var jwtSecret = _configuration["JwtSettings:Secret"];
            var jwtIssuer = _configuration["JwtSettings:Issuer"];
            var jwtAudience = _configuration["JwtSettings:Audience"];

            if (string.IsNullOrEmpty(jwtSecret))
                throw new InvalidOperationException("JWT Secret is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}