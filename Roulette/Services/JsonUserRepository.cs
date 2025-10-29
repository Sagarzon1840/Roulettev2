using System.Text.Json;
using Roulette.Entities;

namespace Roulette.Services
{
    public class JsonUserRepository : IUserRepository
    {
        private readonly string _path;
        private readonly List<User> _items;
        private readonly object _lock = new();

        public JsonUserRepository(IConfiguration config)
        {
            _path = config.GetValue<string>("JsonMock:UsersFile") ?? "data/users.json";
            var dir = Path.GetDirectoryName(_path);
            if (!string.IsNullOrEmpty(dir)) Directory.CreateDirectory(dir);
            if (File.Exists(_path))
                _items = JsonSerializer.Deserialize<List<User>>(File.ReadAllText(_path)) ?? new List<User>();
            else
                _items = new List<User>();
        }

        public Task<User?> FindByIdAsync(Guid id) => Task.FromResult(_items.FirstOrDefault(u => u.Id == id));

        public Task<User?> FindByUsernameAsync(string username) => Task.FromResult(_items.FirstOrDefault(u => u.Username == username));

        public Task<User> CreateAsync(User user)
        {
            lock (_lock)
            {
                if (user.Id == Guid.Empty) user.Id = Guid.NewGuid();
                _items.Add(user);
                File.WriteAllText(_path, JsonSerializer.Serialize(_items));
            }
            return Task.FromResult(user);
        }

        public Task SaveChangesAsync() => Task.CompletedTask;
    }
}
