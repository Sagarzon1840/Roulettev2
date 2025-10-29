using Microsoft.EntityFrameworkCore;
using Npgsql;

namespace Roulette.Data
{
	public static class Database
	{
		public static NpgsqlDataSource CreateDataSource(string? connectionString)
		{
			if (string.IsNullOrWhiteSpace(connectionString))
			{
				throw new InvalidOperationException("Conexión 'Default' no configurada. Ajustar variable de entorno 'ConnectionStrings__Default' o añadirla a appsettings.json bajo nombre 'ConnectionStrings:Default'");
			}

			var builder = new NpgsqlDataSourceBuilder(connectionString);
			return builder.Build(); // NpgsqlDataSource implementa IDisposable/IAsyncDisposable
		}
	}
}




