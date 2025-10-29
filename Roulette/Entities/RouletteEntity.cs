using System.ComponentModel.DataAnnotations;

namespace Roulette.Entities
{
	public class RouletteEntity
	{
		[Key]
		public Guid Id { get; set; }

		public bool IsOpen { get; set; } = false;

		public ICollection<Bet> Bets { get; set; } = new List<Bet>();
	}
}




