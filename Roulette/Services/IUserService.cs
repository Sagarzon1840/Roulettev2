using Roulette.DTOs;
using Roulette.Entities;

namespace Roulette.Services
{
    public interface IUserService
    {
        Task<object> CreateUser(string username, string password);
        Task<object> Login(LoginUsersDTO login);
        Task<User> FindUser(Guid id);
    }
}