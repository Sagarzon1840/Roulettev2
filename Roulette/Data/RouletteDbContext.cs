using Microsoft.EntityFrameworkCore;
using Roulette.Entities;

namespace Roulette.Data
{
	public class RouletteDbContext : DbContext
	{
		public RouletteDbContext(DbContextOptions<RouletteDbContext> options) : base(options)
		{
		}

		public DbSet<User> Users => Set<User>();
		public DbSet<RouletteEntity> Roulettes => Set<RouletteEntity>();
		public DbSet<Bet> Bets => Set<Bet>();

		protected override void OnModelCreating(ModelBuilder modelBuilder)
		{
			modelBuilder.Entity<User>(entity =>
			{
				entity.HasMany(u => u.Bets)
					.WithOne(b => b.User)
					.HasForeignKey(b => b.UserId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			modelBuilder.Entity<RouletteEntity>(entity =>
			{
				entity.HasMany(r => r.Bets)
					.WithOne(b => b.Roulette)
					.HasForeignKey(b => b.RouletteId)
					.OnDelete(DeleteBehavior.Cascade);
			});

			base.OnModelCreating(modelBuilder);
		}
	}
}




