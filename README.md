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
## Local sin Docker
```bash
cd Labotec.Api
dotnet restore
dotnet run -c Release
# API: http://localhost:8080/swagger
```
