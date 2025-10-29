using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using System.Security.Claims;
using Roulette.DTOs;
using Roulette.Entities;

namespace Roulette.Services
{
    public class UserServiceProxy : IUserService
    {
        private readonly UserService _inner;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IAuthorizationService _authorization;

        public UserServiceProxy(UserService inner, IHttpContextAccessor httpContextAccessor, IAuthorizationService authorization)
        {
            _inner = inner;
            _httpContextAccessor = httpContextAccessor;
            _authorization = authorization;
        }

        private ClaimsPrincipal? User => _httpContextAccessor.HttpContext?.User;

        public async Task<object> CreateUser(string username, string password)
        {
            // Allow anonymous signup (no authentication) to create a new user.
            var ctxUser = User;
            var isAuthenticated = ctxUser?.Identity?.IsAuthenticated ?? false;

            if (!isAuthenticated)
            {
                return await _inner.CreateUser(username, password);
            }

            // If request is authenticated, require Admin role to create other users
            if (!ctxUser!.IsInRole("Admin"))
                throw new UnauthorizedAccessException("No autorizado para crear usuarios.");

            return await _inner.CreateUser(username, password);
        }

        public async Task<object> Login(LoginUsersDTO login)
        {
            // Allow login anonymously
            return await _inner.Login(login);
        }

        public async Task<User> FindUser(Guid id)
        {
            var ctxUser = User;
            if (ctxUser == null || !(ctxUser.Identity?.IsAuthenticated ?? false))
            {
                // require token
                throw new UnauthorizedAccessException("Usuario no autenticado");
            }

            // Allow if admin or if the token's id matches the requested id or policy allows
            var authorized = ctxUser.IsInRole("Admin") ||
                             ctxUser.HasClaim(c => c.Type == "id" && c.Value == id.ToString()) ||
                             (await _authorization.AuthorizeAsync(ctxUser!, null, "ReadUsersPolicy")).Succeeded;

            if (!authorized) throw new UnauthorizedAccessException("Usuario no autorizado para lectura");

            return await _inner.FindUser(id);
        }
    }
}