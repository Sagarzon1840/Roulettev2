using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Entities;

namespace Roulette.Services
{
    public class EfUserRepository : IUserRepository
    {
        private readonly RouletteDbContext _dbContext;

        public EfUserRepository(RouletteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<User?> FindByIdAsync(Guid id)
        {
            return await _dbContext.Users.Include(u => u.Bets).FirstOrDefaultAsync(u => u.Id == id);
        }

        public async Task<User?> FindByUsernameAsync(string username)
        {
            return await _dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
        }

        public async Task<User> CreateAsync(User user)
        {
            _dbContext.Users.Add(user);
            await _dbContext.SaveChangesAsync();
            return user;
        }

        public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}
