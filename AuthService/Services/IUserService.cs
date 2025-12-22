using AuthService.Models;

namespace AuthService.Services
{
    public interface IUserService
    {
        Task<User> CreateUserAsync(string username, string email, string password);
        Task<string?> LoginAsync(string email, string password);
    }
}