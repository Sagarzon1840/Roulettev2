using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.Entities;

namespace Roulette.Services
{
    public class EfRouletteRepository : IRouletteRepository
    {
        private readonly RouletteDbContext _dbContext;

        public EfRouletteRepository(RouletteDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<RouletteEntity> CreateAsync(RouletteEntity roulette)
        {
            _dbContext.Roulettes.Add(roulette);
            await _dbContext.SaveChangesAsync();
            return roulette;
        }

        public async Task<RouletteEntity?> GetByIdAsync(Guid id, bool includeBets = false, bool includeBetsWithUsers = false)
        {
            IQueryable<RouletteEntity> query = _dbContext.Roulettes;
            if (includeBets)
            {
                query = query.Include(r => r.Bets);
                if (includeBetsWithUsers)
                    query = query.Include(r => r.Bets).ThenInclude(b => b.User);
            }
            return await query.FirstOrDefaultAsync(r => r.Id == id);
        }

        public async Task AddBetAsync(Bet bet)
        {
            _dbContext.Bets.Add(bet);
            await _dbContext.SaveChangesAsync();
        }

        public Task SaveChangesAsync() => _dbContext.SaveChangesAsync();
    }
}
