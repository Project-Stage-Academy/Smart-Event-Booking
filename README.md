# Smart Event Booking

ASP.NET Core REST API and Angular application for event discovery and ticket booking.

For product vision, scope, and contribution workflow, see `PROJECT_SCOPE.md`.

## Prerequisites

- .NET SDK 10.0
- SQL Server (local instance or Docker)
- Node.js (v18 or later) and npm
- Angular CLI (`npm install -g @angular/cli`)

## 1) Configure the database settings

This app reads database values from environment variables (`Database__Server`, `Database__Name`, `Database__User`, `Database__Password`, `Database__TrustServerCertificate`) and falls back to `src/SmartEventBooking.Web/appsettings.json`.

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
dotnet ef database update --project src/SmartEventBooking.Infrastructure/SmartEventBooking.Infrastructure.csproj --startup-project src/SmartEventBooking.Web/SmartEventBooking.Web.csproj --context ApplicationDbContext
```

## 4) Run the app

```bash
dotnet run --project src/SmartEventBooking.Web/SmartEventBooking.Web.csproj --launch-profile https
```

Default URLs (development):

- `https://localhost:7134`
- `http://localhost:5270`

If you run without the `https` launch profile, you may see:

- `Failed to determine the https port for redirect.`

Use `--launch-profile https` (shown above), or set `ASPNETCORE_URLS` to include an HTTPS URL.

## Run the Frontend (Development server)

To start a local development server for the frontend, navigate to the client directory and run:

```bash
cd src/client
ng serve
```

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
