# Labotec API (standalone)
- .NET 8 + MySQL + Identity/JWT + Swagger + Serilog
- Storage: File (local) o Azure Blob (por config)
- 100% independiente del frontend

## Docker (API + MySQL)
```bash
docker compose -f docker-compose.api.yml build
docker compose -f docker-compose.api.yml up -d
# API: http://localhost:8080/swagger
# MySQL host local: 127.0.0.1:3307 (labotec / SuperClave!2025)
# Usuario seed: admin / Admin#2025!
```

### Autenticación desde Swagger
1. Abre `POST /api/Auth/login` y ejecuta la petición con las credenciales seed `admin` / `Admin#2025!`.
2. Copia el valor de `token` devuelto por el login.
3. Pulsa el botón **Authorize** en la esquina superior derecha de Swagger UI e ingresa `Bearer {token}`.
4. Las autorizaciones quedan guardadas para la sesión actual para evitar reenviar el token en cada recarga.

## Local sin Docker
```bash
cd Labotec.Api
dotnet restore
dotnet run -c Release
# API: http://localhost:8080/swagger
```
