# Smart Event Booking

Full-stack event discovery and ticket booking platform — ASP.NET Core 10 API + Angular client.

For product vision, scope, and contribution workflow, see `PROJECT_SCOPE.md`.

## Prerequisites

**To run with Docker Compose:**
- [Docker Desktop](https://www.docker.com/products/docker-desktop/)

**To run locally:**
- .NET SDK 10.0
- Node.js 24 LTS (see [Node version setup](#node-version-setup))
- SQL Server (local instance or Docker)

## Running with Docker Compose

The quickest way to run the full stack (API + client + database) without any local tooling beyond Docker.

1. Copy the env file and fill in your credentials:
   ```bash
   cp .env.example .env
   ```

2. Start everything:
   ```bash
   docker compose up -d --build
   ```

   | Service | URL |
   |---|---|
   | Client (Angular) | http://localhost:4200 |
   | API (ASP.NET Core) | http://localhost:5270 |
   | SQL Server | localhost:1433 |

   Migrations run automatically on API startup — no manual `dotnet ef` step needed.

3. Stop everything:
   ```bash
   docker compose down
   ```
   Add `-v` to also wipe the database volume.

---

## Running locally

Follow the numbered steps below to run the API and client separately.

## 1) Configure the database settings

This app reads database values from environment variables (`Database__Server`, `Database__Name`, `Database__User`, `Database__Password`, `Database__TrustServerCertificate`) and falls back to `src/api/SmartEventBooking.Web/appsettings.json`.

Admin seeding credentials are also read from environment variables:

- `AdminSettings__Email`
- `AdminSettings__Password`

Use one of these options:

- Set environment variables directly in your shell.
- Or copy `.env.example` to `.env`. The app auto-loads `.env` on startup (searches current and parent directories) and only applies values for variables not already set in the shell.

Example:

```bash
cp .env.example .env
```

## 2) Start SQL Server (Docker option)

```bash
docker run -e "ACCEPT_EULA=Y" -e "MSSQL_SA_PASSWORD=YourStrong!Passw0rd" -p 1433:1433 --name smart-event-sql -d mcr.microsoft.com/mssql/server:2022-latest
```

Important: keep `.env` and SQL Server credentials aligned. If the container is started with `MSSQL_SA_PASSWORD=YourStrong!Passw0rd`, set `Database__Password=YourStrong!Passw0rd` in `.env`.

## 3) Create/update the database schema

From the repository root:

```bash
dotnet restore
dotnet ef database update --project src/api/SmartEventBooking.Infrastructure/SmartEventBooking.Infrastructure.csproj --startup-project src/api/SmartEventBooking.Web/SmartEventBooking.Web.csproj --context ApplicationDbContext
```

## 4) Run the API

```bash
dotnet run --project src/api/SmartEventBooking.Web/SmartEventBooking.Web.csproj --launch-profile https
```

Default URLs (development):

- `https://localhost:7134`
- `http://localhost:5270`
- Swagger UI: `https://localhost:7134/swagger`

If you run without the `https` launch profile, you may see:

- `Failed to determine the https port for redirect.`

Use `--launch-profile https` (shown above), or set `ASPNETCORE_URLS` to include an HTTPS URL.

## 5) Run the client

```bash
cd src/client
npm ci
ng serve
```

Client runs at `http://localhost:4200`. The API URL it connects to is configured in `src/client/public/assets/config.json`.

## 6) Verify connectivity

Health endpoint:

- `GET /health/db`
- Example: `https://localhost:7134/health/db`

Expected result:

- `200 OK` with `{ "status": "ok", "database": "reachable" }`
- If you get `503 Service Unavailable`, verify DB credentials and whether shell environment variables are overriding `.env` values.

## Optional commands

```bash
dotnet build
dotnet test
```

---

## Node version setup

The client requires **Node.js 24 LTS**, pinned in `src/client/.nvmrc`.

### Option A — Install Node 24 directly (simplest)

Download and install Node 24 LTS from [nodejs.org](https://nodejs.org/en/download). No version manager needed if you only work on this project.

### Option B — Use fnm (if you work on multiple projects with different Node versions)

**Windows:**
```powershell
winget install Schniz.fnm
```
Add to your PowerShell profile (run once):
```powershell
Add-Content $PROFILE "fnm env --use-on-cd --shell power-shell | Out-String | Invoke-Expression"
```
Restart PowerShell, then inside `src/client/`:
```powershell
fnm install
fnm use
```

**macOS / Linux:**
```bash
curl -fsSL https://fnm.vercel.app/install | bash
```
Then inside `src/client/`:
```bash
fnm install
fnm use
```

> **Switching versions and seeing native module errors?** Delete `node_modules` and run `npm ci` again — native binaries are version-specific.
