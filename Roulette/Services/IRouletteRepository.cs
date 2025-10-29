using Roulette.Entities;

namespace Roulette.Services
{
    public interface IRouletteRepository
    {
        Task<RouletteEntity> CreateAsync(RouletteEntity roulette);
        Task<RouletteEntity?> GetByIdAsync(Guid id, bool includeBets = false, bool includeBetsWithUsers = false);
        Task AddBetAsync(Bet bet);
        Task SaveChangesAsync();
    }
}
