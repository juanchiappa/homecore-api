# HomeCore API

A self-hosted REST API built with ASP.NET Core 8 for monitoring and managing any Docker-based homelab.
No hardcoded services — define what to monitor in a JSON file without touching any code.

## Features

- **Plugin-based monitor system**: HTTP health-check and Docker container status included. Easily extensible with custom monitors.
- **JWT Bearer authentication** to protect all endpoints.
- **Swagger UI** available in development mode.
- **100% external config**: `services.json` defines what to monitor in your homelab.
- **Docker-ready**: single `docker compose up` and you're running.

## Quick Start

### 1. Clone and configure

```bash
git clone https://github.com/yourusername/homecore-api.git
cd homecore-api
```

Create your secrets file:

```bash
cp .env.example .env
# Edit .env with your JWT_KEY and ADMIN_PASSWORD
```

Create your services config:

```bash
mkdir -p config
cp HomeCore.API/config/services.example.json config/services.json
# Edit config/services.json with your services
```

### 2. Start with Docker Compose

```bash
docker compose up -d
```

API available at `http://localhost:5000`.

### 3. Authenticate

```bash
curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{ "userName": "admin", "password": "your-password" }'
```

Response:

```json
{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-15T11:00:00Z"
}
```

Use the token in the `Authorization: Bearer <token>` header for all other endpoints.

## Services Configuration

Edit `config/services.json` to define what to monitor:

```json
{
  "services": [
    {
      "name": "Jellyfin",
      "type": "http_healthcheck",
      "url": "http://jellyfin:8096/health"
    },
    {
      "name": "qBittorrent",
      "type": "http_healthcheck",
      "url": "http://qbittorrent:8080"
    },
    {
      "name": "Nextcloud",
      "type": "docker_container",
      "containerName": "nextcloud"
    }
  ]
}
```

### Available monitor types

| Type | Description | Required field |
|---|---|---|
| `http_healthcheck` | GET request to URL, healthy if 2xx | `url` |
| `docker_container` | Container state via Docker Engine API | `containerName` |

## Endpoints

| Method | Route | Description |
|---|---|---|
| `POST` | `/api/auth/login` | Get JWT token |
| `GET` | `/api/services` | Status of services defined in `services.json` |
| `GET` | `/api/containers` | List Docker containers (`?all=true` includes stopped) |
| `GET` | `/api/containers/{id}/stats` | CPU, RAM and network of a container |
| `GET` | `/api/system/metrics` | Host CPU%, RAM% and disk usage |
| `GET` | `/api/system/uptime` | Host uptime |

## Environment Variables

| Variable | Default | Description |
|---|---|---|
| `Jwt__Key` | *(required)* | Secret key for signing JWT tokens |
| `Jwt__ExpirationMinutes` | `60` | Token lifetime in minutes |
| `Admin__UserName` | `admin` | Admin username |
| `Admin__Password` | *(required)* | Admin password |
| `Docker__Uri` | `unix:///var/run/docker.sock` | Docker Engine URI |
| `ServicesConfigPath` | `config/services.json` | Path to services config file |

## Homelab Examples

**Plex + Home Assistant + Portainer:**
```json
{
  "services": [
    { "name": "Plex",           "type": "http_healthcheck", "url": "http://plex:32400/identity" },
    { "name": "Home Assistant", "type": "http_healthcheck", "url": "http://homeassistant:8123" },
    { "name": "Portainer",      "type": "docker_container", "containerName": "portainer" }
  ]
}
```

**Full *arr stack:**
```json
{
  "services": [
    { "name": "Sonarr",   "type": "http_healthcheck", "url": "http://sonarr:8989/ping" },
    { "name": "Radarr",   "type": "http_healthcheck", "url": "http://radarr:7878/ping" },
    { "name": "Prowlarr", "type": "http_healthcheck", "url": "http://prowlarr:9696/ping" },
    { "name": "Bazarr",   "type": "docker_container", "containerName": "bazarr" }
  ]
}
```

## Tech Stack

| Component | Technology |
|---|---|
| Framework | ASP.NET Core 8 |
| Authentication | JWT Bearer (HS256) |
| Persistence | Dapper + SQLite |
| Docker integration | Docker.DotNet |
| Documentation | Swashbuckle (Swagger UI) |
| Testing | xUnit + Moq |
| Deployment | Docker + Docker Hub |

## Project Structure
HomeCore.Entities/   → Shared entities and DTOs
HomeCore.DAL/        → Repositories, Dapper, SQLite
HomeCore.BLL/        → Services, business logic, monitor plugin system
HomeCore.API/        → Controllers, ASP.NET Core configuration
HomeCore.Tests/      → Unit tests (xUnit + Moq)

## License

MIT