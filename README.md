# Roulettev2

Proyecto: Roulette (Backend) — .NET 8

## Descripción
Instrucciones básicas para ejecutar y probar localmente el proyecto Roulettev2.

## Requisitos
- .NET 8 SDK instalado
- Git
- (Opcional) Visual Studio 2022/2023 o VS Code
- Repo: https://github.com/Sagarzon1840/Roulettev2.git

## Abrir el proyecto
- Visual Studio: abrir la solución en `Roulette\Roulette.csproj` y ejecutar (F5).
- VS Code / terminal: abrir la carpeta del proyecto y usar la CLI (`dotnet run` desde el directorio del proyecto).

## Ejecutar usando el mock JSON (recomendado para desarrollo local sin PostgreSQL)
El proyecto incluye variables para desarrollo en `Properties/launchSettings.json`:
- `USE_JSON_MOCK = true`
- `Jwt__Secret = "fD4x9sK8qTzB3vH7uYw1pR0eLmN6aQ2z"`

Con `USE_JSON_MOCK=true` la aplicación utilizará ficheros JSON locales como almacenamiento simulado en lugar de PostgreSQL. Esto es útil para desarrollo y pruebas locales.

## Ficheros de datos mock
- Usuarios: `Roulette/data/users.json`
- Ruletas: `Roulette/data/roulettes.json`

El mock JSON se actualizará automáticamente al crear usuarios, ruletas y al colocar apuestas.

## Swagger / probar endpoints
- Abre: `http://localhost:<puerto>/swagger` (el puerto se muestra al arrancar la aplicación).

Flujo típico para probar la aplicación:
1. `POST /user/signin` (body JSON: `{ "username":"alice","password":"password123" }`) → devuelve token JWT.
2. En Swagger, pulsar el botón *Authorize* → introducir `Bearer <token>` → *Authorize*.
3. Llamar endpoints protegidos (por ejemplo: `POST /roulette/create`, `GET /roulette/{id}`, `GET /user/{id}`).

Ejemplo curl (login):
```
curl -X POST http://localhost:5237/user/login -H "Content-Type: application/json" -d '{"username":"alice","password":"password123"}'
```

## Notas
- Las variables en `launchSettings.json` facilitan el desarrollo local; revisa y ajusta puertos si es necesario.
- La clave `Jwt__Secret` es un valor de ejemplo incluido para desarrollo. No usarla en producción.