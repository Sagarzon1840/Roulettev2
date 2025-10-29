using System.Text.Json;
using Roulette.Entities;

namespace Roulette.Services
{
    public class JsonRouletteRepository : IRouletteRepository
    {
        private readonly string _path;
        private readonly List<RouletteEntity> _items;
        private readonly object _lock = new();

        public JsonRouletteRepository(IConfiguration config)
        {
            _path = config.GetValue<string>("JsonMock:RoulettesFile") ?? "data/roulettes.json";
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            if (File.Exists(_path))
                _items = JsonSerializer.Deserialize<List<RouletteEntity>>(File.ReadAllText(_path)) ?? new List<RouletteEntity>();
            else
                _items = new List<RouletteEntity>();
        }

        public Task<RouletteEntity> CreateAsync(RouletteEntity roulette)
        {
            lock (_lock)
            {
                if (roulette.Id == Guid.Empty) roulette.Id = Guid.NewGuid();
                _items.Add(roulette);
                File.WriteAllText(_path, JsonSerializer.Serialize(_items));
            }
            return Task.FromResult(roulette);
        }

        public Task<RouletteEntity?> GetByIdAsync(Guid id, bool includeBets = false, bool includeBetsWithUsers = false)
        {
            return Task.FromResult(_items.FirstOrDefault(r => r.Id == id));
        }

        public Task AddBetAsync(Bet bet)
        {
            lock (_lock)
            {
                var roulette = _items.FirstOrDefault(r => r.Id == bet.RouletteId);
                if (roulette != null)
                {
                    if (roulette.Bets == null) roulette.Bets = new List<Bet>();
                    roulette.Bets.Add(bet);
                    File.WriteAllText(_path, JsonSerializer.Serialize(_items));
                }
            }
            return Task.CompletedTask;
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
