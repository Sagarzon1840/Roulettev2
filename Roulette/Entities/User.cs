using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roulette.Entities
{
	public class User
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		public string Username { get; set; } = string.Empty;

		[Required]
		public string Password { get; set; } = string.Empty;

		public int Credit { get; set; } = 10000;

		public ICollection<Bet> Bets { get; set; } = new List<Bet>();
	}
}




