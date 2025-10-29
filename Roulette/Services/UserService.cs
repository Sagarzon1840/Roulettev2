using Microsoft.IdentityModel.Tokens;
using Roulette.DTOs;
using Roulette.Entities;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace Roulette.Services
{
	public class UserService
	{
		private readonly IUserRepository _repository;
		private readonly IConfiguration _configuration;

		public UserService(IUserRepository repository, IConfiguration configuration)
		{
			_repository = repository;
			_configuration = configuration;
		}

		public async Task<object> CreateUser(string username, string password)
		{
			var user = new User
			{
				Username = username,
				Password = password,
				Credit = 10000
			};

			var result = await _repository.CreateAsync(user);

			// Create token like in Login
			var secret = _configuration["Jwt:Secret"];
			if (string.IsNullOrWhiteSpace(secret))
			{
				throw new InvalidOperationException("JWT secret no configurado");
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim("id", result.Id.ToString()),
					new Claim(ClaimTypes.Name, result.Username)
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			return new
			{
				message = "¡Usuario creado y registrado!",
				id = result.Id,
				username = result.Username,
				token = tokenString
			};
		}

		public async Task<object> Login(LoginUsersDTO login)
		{
			var user = await _repository.FindByUsernameAsync(login.Username);
			if (user == null)
			{
				throw new InvalidOperationException($"Usuario {login.Username} no encontrado");
			}
			if (user.Password != login.Password)
			{
				throw new InvalidOperationException("Credenciales incorrectas");
			}

			var secret = _configuration["Jwt:Secret"];
			if (string.IsNullOrWhiteSpace(secret))
			{
				throw new InvalidOperationException("JWT secret no configurado");
			}

			var tokenHandler = new JwtSecurityTokenHandler();
			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
			var tokenDescriptor = new SecurityTokenDescriptor
			{
				Subject = new ClaimsIdentity(new[]
				{
					new Claim("id", user.Id.ToString()),
					new Claim(ClaimTypes.Name, user.Username)
				}),
				Expires = DateTime.UtcNow.AddHours(1),
				SigningCredentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256Signature)
			};
			var token = tokenHandler.CreateToken(tokenDescriptor);
			var tokenString = tokenHandler.WriteToken(token);

			return new
			{
				message = "¡Inicio de sesión exitoso!",
				id = user.Id,
				username = user.Username,
				token = tokenString
			};
		}

		public async Task<User> FindUser(Guid id)
		{
			var user = await _repository.FindByIdAsync(id);
			if (user == null)
			{
				throw new KeyNotFoundException($"Usuario con id {id} no encontrado");
			}
			return user;
		}
	}
}




