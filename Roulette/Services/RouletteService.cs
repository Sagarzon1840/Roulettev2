using Microsoft.EntityFrameworkCore;
using Roulette.Data;
using Roulette.DTOs;
using Roulette.Entities;

namespace Roulette.Services
{
	public class RouletteService
	{
		private readonly IUserRepository _userRepository;
		private readonly IRouletteRepository _rouletteRepository;

		public RouletteService(IUserRepository userRepository, IRouletteRepository rouletteRepository)
		{
			_userRepository = userRepository;
			_rouletteRepository = rouletteRepository;
		}

		public async Task<string> Create()
		{
			var roulette = new RouletteEntity();
			await _rouletteRepository.CreateAsync(roulette);
			return $"Creación de ruleta con id: {roulette.Id}";
		}

		public async Task<string> Open(Guid rouletteId)
		{
			var roulette = await _rouletteRepository.GetByIdAsync(rouletteId);
			if (roulette != null && !roulette.IsOpen)
			{
				roulette.IsOpen = true;
				await _rouletteRepository.SaveChangesAsync();
				return $"Activación de ruleta con id: {rouletteId}";
			}
			return "Activación de ruleta fallida";
		}

		public async Task<string> PlaceBet(Guid rouletteId, Guid userId, BetCreationDTO bet)
		{
			var user = await _userRepository.FindByIdAsync(userId);
			if (bet.Number == null && string.IsNullOrWhiteSpace(bet.Color))
				throw new InvalidOperationException("Elegir color o número");

			var roulette = await _rouletteRepository.GetByIdAsync(rouletteId, includeBets: true);
			if (roulette == null) throw new InvalidOperationException("Ruleta no encontrada");

			if (bet.Number.HasValue && !string.IsNullOrEmpty(bet.Color))
				throw new InvalidOperationException("No es posible seleccionar número y color");

			if (user == null) throw new InvalidOperationException("Usuario no encontrado");

			if (user.Credit >= bet.Amount && roulette.IsOpen)
			{
				user.Credit -= bet.Amount;
				var newBet = new Bet
				{
					Amount = bet.Amount,
					Number = bet.Number,
					Color = bet.Color,
					UserId = user.Id,
					RouletteId = roulette.Id
				};
				await _rouletteRepository.AddBetAsync(newBet);
				await _userRepository.SaveChangesAsync();
				return $"Apuesta subida exitosamente con número: {bet.Number}";
			}
			return "Error en la apuesta, verificar créditos o si la apuesta está abierta";
		}

		public async Task<object> Close(Guid rouletteId)
		{
			var roulette = await _rouletteRepository.GetByIdAsync(rouletteId, includeBets: true, includeBetsWithUsers: true);
			if (roulette == null) throw new InvalidOperationException("Ruleta no encontrada");

			if (roulette.IsOpen)
			{
				var winningNumber = Random.Shared.Next(0, 37);
				var winningColor = winningNumber % 2 == 0 ? "rojo" : "negro";

				roulette.IsOpen = false;

				var results = new List<object>();

				foreach (var bet in roulette.Bets ?? Enumerable.Empty<Bet>())
				{
					var user = await _userRepository.FindByIdAsync(bet.UserId);
					if (user == null) continue;
					if (bet.Number == winningNumber)
					{
						user.Credit += bet.Amount * 5;
						results.Add(new { userId = user.Id, resultValue = "win", betMumber = bet.Number, winningNumber, winningColor, amount = bet.Amount * 5, newUserAmount = user.Credit });
					}
					else if (bet.Color == winningColor)
					{
						user.Credit += (int)Math.Round(bet.Amount * 1.8);
						results.Add(new { userId = user.Id, resultValue = "win", betMumber = bet.Number, winningNumber, amount = (int)Math.Round(bet.Amount * 1.8), newUserAmount = user.Credit });
					}
					else
					{
						results.Add(new { userId = user.Id, resultValue = "lose", betMumber = bet.Number, winningNumber, amount = -bet.Amount, newUserAmount = user.Credit });
					}
				}

				await _rouletteRepository.SaveChangesAsync();
				await _userRepository.SaveChangesAsync();
				return new { result = results };
			}

			return new { result = Array.Empty<object>() };
		}

		public async Task<RouletteEntity> FindRoulette(Guid id)
		{
			var roulette = await _rouletteRepository.GetByIdAsync(id, includeBets: true);
			if (roulette == null)
			{
				throw new KeyNotFoundException($"Ruleta con id: {id} no encontrada");
			}
			return roulette;
		}
	}
}







