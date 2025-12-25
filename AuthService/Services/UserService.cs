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

            if (string.IsNullOrEmpty(jwtSecret))
                throw new InvalidOperationException("JWT Secret is not configured.");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.Username),
                new Claim(JwtRegisteredClaimNames.Email, user.Email)
            };

            var token = new JwtSecurityToken(
                claims: claims,
                expires: DateTime.UtcNow.AddHours(2),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        #endregion
    }
}