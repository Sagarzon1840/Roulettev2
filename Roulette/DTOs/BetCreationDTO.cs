using System.ComponentModel.DataAnnotations;

namespace Roulette.DTOs
{
	public class BetCreationDTO
	{
		[RegularExpression("^(rojo|negro)$", ErrorMessage = "Color debe ser rojo o negro")]
		public string? Color { get; set; }

		[Range(0, 36)]
		public int? Number { get; set; }

		[Range(1, 10000)]
		public int Amount { get; set; }
	}
}




