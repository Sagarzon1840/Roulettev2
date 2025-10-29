using Microsoft.EntityFrameworkCore;
using Npgsql;
using Roulette.Services;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

var builder = WebApplication.CreateBuilder(args);
var configuration = builder.Configuration;
var logger = LoggerFactory.Create(lb => lb.AddConsole()).CreateLogger("Program");

// Prefer configuration (which picks up launchSettings) for the mock flag; fall back to environment variable
var useJsonMock = configuration.GetValue<bool?>("USE_JSON_MOCK")
                  ?? (Environment.GetEnvironmentVariable("USE_JSON_MOCK") == "true");
logger.LogInformation("USE_JSON_MOCK={useJsonMock}", useJsonMock);

// Condicional: usar Json mock o EF repository
if (useJsonMock)
{
	builder.Services.AddSingleton<IUserRepository, JsonUserRepository>();
	builder.Services.AddSingleton<IRouletteRepository, JsonRouletteRepository>();
}
else
{
	var connectionString = configuration.GetValue<string>("ConnectionStrings:Default")
	                   ?? Environment.GetEnvironmentVariable("ConnectionStrings__Default");

	if (string.IsNullOrWhiteSpace(connectionString))
	{
		throw new InvalidOperationException("Conexión 'Default' no configurada. Ajustar variable de entorno 'ConnectionStrings__Default' o añadirla a appsettings.json bajo nombre 'ConnectionStrings:Default'");
	}

	// Registra NpgsqlDataSource como singleton para que controle su ciclo de vida
	builder.Services.AddSingleton<NpgsqlDataSource>(_ =>
	{
		return Roulette.Data.Database.CreateDataSource(connectionString);
	});

	// Registrar fuente en la configuración de DbContext
	builder.Services.AddDbContext<Roulette.Data.RouletteDbContext>((sp, options) =>
	{
		var dataSource = sp.GetRequiredService<NpgsqlDataSource>();
		options.UseNpgsql(dataSource);
	});

	builder.Services.AddScoped<IUserRepository, EfUserRepository>();
	builder.Services.AddScoped<IRouletteRepository, EfRouletteRepository>();
}

// JWT secret de ambiente o configuración (prefer configuration/launchSettings)
var jwtSecret = configuration.GetValue<string>("Jwt__Secret")
                ?? configuration.GetValue<string>("Jwt:Secret")
                ?? Environment.GetEnvironmentVariable("Jwt__Secret");

if (string.IsNullOrWhiteSpace(jwtSecret))
{
	throw new InvalidOperationException("JWT secret no configurado. Ajustar variable 'Jwt__Secret' o añadir 'Jwt:Secret' en appsettings.json / user-secrets.");
}

logger.LogInformation("Using JWT secret: {hasSecret}", !string.IsNullOrEmpty(jwtSecret));

// Autenticación - JWT
builder.Services
	.AddAuthentication("Bearer")
	.AddJwtBearer(options =>
	{
		options.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters
		{
			ValidateIssuer = false,
			ValidateAudience = false,
			ValidateLifetime = true,
			ValidateIssuerSigningKey = true,
			IssuerSigningKey = new Microsoft.IdentityModel.Tokens.SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(jwtSecret))
		};
	});

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
// Configure Swagger to expose JWT Bearer input
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Name = "Authorization",
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "Ingresar Bearer token",
    });

    c.AddSecurityRequirement(new OpenApiSecurityRequirement
    {
        {
            new OpenApiSecurityScheme
            {
                Reference = new OpenApiReference
                {
                    Type = ReferenceType.SecurityScheme,
                    Id = "Bearer"
                }
            },
            Array.Empty<string>()
        }
    });
});

// asegura disponibilidad de IHttpContextAccessor y IAuthorizationService
builder.Services.AddHttpContextAccessor();
builder.Services.AddAuthorization(options =>
{
    // definir políticas
    options.AddPolicy("ReadUsersPolicy", policy => policy.RequireClaim("scope", "users.read"));
});

// registrar implementación definida y después proxy como IUserService
builder.Services.AddScoped<UserService>(); // implementación concreta
builder.Services.AddScoped<IUserService>(sp =>
{
    var real = sp.GetRequiredService<UserService>();
    return new UserServiceProxy(
        real,
        sp.GetRequiredService<IHttpContextAccessor>(),
        sp.GetRequiredService<Microsoft.AspNetCore.Authorization.IAuthorizationService>());
});

builder.Services.AddTransient<RouletteService>();

var app = builder.Build();

// Registra en log los valores registrados en startup
var appLogger = app.Services.GetRequiredService<ILogger<Program>>();
appLogger.LogInformation("Application environment: {env}", app.Environment.EnvironmentName);
appLogger.LogInformation("USE_JSON_MOCK={useJsonMock}", useJsonMock);

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.Run();
