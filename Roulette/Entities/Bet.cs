using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Roulette.Entities
{
	public class Bet
	{
		[Key]
		public Guid Id { get; set; }

		[Required]
		public int Amount { get; set; }

		public int? Number { get; set; }

		[StringLength(10)]
		public string? Color { get; set; }

		[Required]
		public Guid UserId { get; set; }
		public User User { get; set; } = null!;

		[Required]
		public Guid RouletteId { get; set; }
		public RouletteEntity Roulette { get; set; } = null!;
	}
}




