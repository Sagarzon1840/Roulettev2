using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Caching.Memory;
using Roulette.Entities;
using Roulette.DTOs;

namespace Roulette.Services
{
    public class UserServiceCacheDecorator : IUserService
    {
        private readonly IUserService _inner;
        private readonly IMemoryCache _cache;

        public UserServiceCacheDecorator(IUserService inner, IMemoryCache cache)
        {
            _inner = inner;
            _cache = cache;
        }

        public Task<object> CreateUser(string username, string password)
        {
            // creation invalidates cache for that user id; keep simple and delegate
            return _inner.CreateUser(username, password);
        }

        public Task<object> Login(LoginUsersDTO login)
        {
            return _inner.Login(login);
        }

        public Task<User> FindUser(Guid id)
        {
            return _cache.GetOrCreateAsync<User>($"user:{id}", async entry =>
            {
                entry.AbsoluteExpirationRelativeToNow = TimeSpan.FromMinutes(5);
                return await _inner.FindUser(id);
            });
        }
    }
}