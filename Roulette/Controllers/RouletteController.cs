using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roulette.DTOs;
using Roulette.Services;

namespace Roulette.Controllers
{
	[ApiController]
	[Route("roulette")]
	[Authorize]
	public class RouletteController : ControllerBase
	{
		private readonly RouletteService _rouletteService;

		public RouletteController(RouletteService rouletteService)
		{
			_rouletteService = rouletteService;
		}

		[HttpPost("create")]
		public async Task<IActionResult> CreateRoulette()
		{
			var result = await _rouletteService.Create();
			return Ok(result);
		}

		[HttpPut("open/{rouletteId}")]
		public async Task<IActionResult> OpenRoulette([FromRoute] Guid rouletteId)
		{
			var result = await _rouletteService.Open(rouletteId);
			return Ok(result);
		}

		[HttpPost("bet/{rouletteId}")]
		public async Task<IActionResult> PlaceBet([FromRoute] Guid rouletteId, [FromHeader(Name="userid")] Guid userId, [FromBody] BetCreationDTO bet)
		{
			var result = await _rouletteService.PlaceBet(rouletteId, userId, bet);
			return Ok(result);
		}

		[HttpGet("close/{rouletteId}")]
		public async Task<IActionResult> CloseRoulette([FromRoute] Guid rouletteId)
		{
			var result = await _rouletteService.Close(rouletteId);
			return Ok(result);
		}

		[HttpGet("{id}")]
		[AllowAnonymous]
		public async Task<IActionResult> FindRoulette([FromRoute] Guid id)
		{
			var roulette = await _rouletteService.FindRoulette(id);
			return Ok(roulette);
		}
	}
}




