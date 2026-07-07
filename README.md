# HomeCore API

REST API en ASP.NET Core 8 para monitorear y gestionar cualquier homelab basado en Docker.
No hay servicios hardcodeados — definís qué monitorear en un archivo JSON sin tocar código.

## Inicio rápido

### 1. Clonar y configurar

git clone https://github.com/tuusuario/homecore-api.git
cd homecore-api

Crear el archivo de secretos:

cp .env.example .env
# Editar .env con tu JWT_KEY y ADMIN_PASSWORD

Crear tu config de servicios:

mkdir -p config
cp HomeCore.API/config/services.example.json config/services.json
# Editar config/services.json con tus servicios

### 2. Levantar con Docker Compose

docker compose up -d

La API queda disponible en http://localhost:5000.

### 3. Autenticarse

curl -X POST http://localhost:5000/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{ "userName": "admin", "password": "tu-password" }'

Respuesta:

{
  "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAt": "2025-01-15T11:00:00Z"
}

Usá el token en el header Authorization: Bearer <token> para el resto de los endpoints.

## Configuración de servicios

Editá config/services.json:

{
  "services": [
    {
      "name": "Jellyfin",
      "type": "http_healthcheck",
      "url": "http://jellyfin:8096/health"
    },
    {
      "name": "Nextcloud",
      "type": "docker_container",
      "containerName": "nextcloud"
    }
  ]
}

### Tipos de monitor disponibles

| Tipo               | Descripción                        | Campo requerido   |
|--------------------|------------------------------------|-------------------|
| http_healthcheck   | GET al URL, healthy si responde 2xx| url               |
| docker_container   | Estado del contenedor vía Docker   | containerName     |

## Endpoints

| Método | Ruta                         | Descripción                              |
|--------|------------------------------|------------------------------------------|
| POST   | /api/auth/login              | Obtener JWT                              |
| GET    | /api/services                | Estado de los servicios en services.json |
| GET    | /api/containers              | Lista de contenedores Docker             |
| GET    | /api/containers/{id}/stats   | CPU, RAM y red de un contenedor          |
| GET    | /api/system/metrics          | CPU%, RAM% y disco% del host             |
| GET    | /api/system/uptime           | Uptime del host                          |

## Variables de entorno

| Variable                | Default               | Descripción                        |
|-------------------------|-----------------------|------------------------------------|
| Jwt__Key                | (requerido)           | Clave secreta para firmar JWT      |
| Jwt__ExpirationMinutes  | 60                    | Vida útil del token en minutos     |
| Admin__UserName         | admin                 | Nombre del usuario admin           |
| Admin__Password         | (requerido)           | Contraseña del admin               |
| Docker__Uri             | unix:///var/run/docker.sock | URI del Docker Engine        |
| ServicesConfigPath      | config/services.json  | Path al archivo de servicios       |

## Tech stack

| Componente       | Tecnología             |
|------------------|------------------------|
| Framework        | ASP.NET Core 8         |
| Autenticación    | JWT Bearer (HS256)     |
| Persistencia     | Dapper + SQLite        |
| Docker           | Docker.DotNet          |
| Documentación    | Swagger UI             |
| Testing          | xUnit + Moq            |

## Licencia

MIT