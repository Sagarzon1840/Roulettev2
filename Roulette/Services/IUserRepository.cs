using Roulette.Entities;

namespace Roulette.Services
{
    public interface IUserRepository
    {
        Task<User?> FindByIdAsync(Guid id);
        Task<User?> FindByUsernameAsync(string username);
        Task<User> CreateAsync(User user);
        Task SaveChangesAsync();
    }
}
