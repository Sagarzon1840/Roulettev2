using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Roulette.DTOs;
using Roulette.Services;

namespace Roulette.Controllers
{
	[ApiController]
	[Route("user")]
	public class UserController : ControllerBase
	{
		private readonly IUserService _userService;

		public UserController(IUserService userService)
		{
			_userService = userService;
		}

		[HttpPost("signin")]
		[AllowAnonymous]
		public async Task<IActionResult> CreateUser([FromBody] LoginUsersDTO dto)
		{
			var result = await _userService.CreateUser(dto.Username, dto.Password);
			return Ok(result);
		}

		[HttpPost("login")]
		[AllowAnonymous]
		public async Task<IActionResult> Login([FromBody] LoginUsersDTO dto)
		{
			var result = await _userService.Login(dto);
			return Ok(result);
		}

		[HttpGet("{id}")]
		[Authorize]
		public async Task<IActionResult> FindUser([FromRoute] Guid id)
		{
			var user = await _userService.FindUser(id);
			// remove password before returning
			user.Password = null!;
			return Ok(user);
		}
	}
}




