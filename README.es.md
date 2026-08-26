<!-- ══════════════════════════ PORTADA ══════════════════════════ -->
<div align="center">
  <img src="docs/title-banner.svg" width="100%" alt="tasks-api-dotnet"/>
</div>

<!-- ══════════════════════ IDIOMAS / LANGUAGES ══════════════════════ -->
<div align="center">
<a href="README.md"><img src="https://img.shields.io/badge/Português-555555?style=for-the-badge" alt="Português"/></a>
<a href="README.en.md"><img src="https://img.shields.io/badge/English-555555?style=for-the-badge" alt="English"/></a>
<a href="README.es.md"><img src="https://img.shields.io/badge/Español-1987F0?style=for-the-badge" alt="Español"/></a>
</div>

<h1 align="center">tasks-api-dotnet</h1>
<p align="center"><em>Minimal API .NET limpia y bien probada para gestión de tareas</em></p>
<p align="center"><strong>ASP.NET Core Minimal API → EF Core InMemory → xUnit</strong></p>

<div align="center">
<a href="https://github.com/geoggrigori/tasks-api-dotnet/actions/workflows/ci.yml"><img src="https://github.com/geoggrigori/tasks-api-dotnet/actions/workflows/ci.yml/badge.svg" alt="CI"/></a>
<img src="https://img.shields.io/badge/.NET_10-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt="dotnet"/>
<img src="https://img.shields.io/badge/C%23_12-239120?style=flat-square&logo=csharp&logoColor=white" alt="csharp"/>
<img src="https://img.shields.io/badge/Swagger-85EA2D?style=flat-square&logo=swagger&logoColor=black" alt="swagger"/>
<img src="https://img.shields.io/badge/License-MIT-2E7D32?style=flat-square" alt="license"/>
</div>

<div align="center">
<a href="#acerca-de"><img src="https://img.shields.io/badge/▸_ACERCA_DE-1987F0?style=for-the-badge" alt="acerca"/></a>
<a href="#endpoints"><img src="https://img.shields.io/badge/▸_ENDPOINTS-000000?style=for-the-badge" alt="endpoints"/></a>
<a href="#flujo-de-la-petición"><img src="https://img.shields.io/badge/▸_FLUJO-1987F0?style=for-the-badge" alt="flujo"/></a>
<a href="#uso"><img src="https://img.shields.io/badge/▸_USO-000000?style=for-the-badge" alt="uso"/></a>
</div>

<br/>

> 📘 **Swagger UI incluido** — corre la API y abre `/swagger` para explorar todo interactivamente.

## Acerca de

Una **Minimal API** limpia y bien probada para gestión de tareas, construida con **ASP.NET Core**, **EF Core** y **xUnit**.

**Destacados:**
- CRUD completo de tareas vía interfaz REST.
- Filtro opcional `?done=true|false` al listar.
- Validación de entrada: títulos vacíos se rechazan con `400`.
- Semántica REST correcta: `201 Created` con header `Location`, `204 No Content` en delete, `404 Not Found` para ids desconocidos.
- Abstracción de repositorio (`ITaskRepository`) sobre el provider EF Core InMemory.
- Documentación interactiva **Swagger UI** / OpenAPI.
- Pruebas de integración con `WebApplicationFactory` cubriendo cada endpoint.

## Endpoints

| Método | Ruta | Descripción | Status |
|---|---|---|---|
| GET | `/tasks` | Lista tareas (opcional `?done=true\|false`) | `200` |
| GET | `/tasks/{id}` | Busca una tarea por id | `200`, `404` |
| POST | `/tasks` | Crea una nueva tarea | `201`, `400` |
| PUT | `/tasks/{id}` | Reemplaza una tarea existente | `200`, `400`, `404` |
| PATCH | `/tasks/{id}/toggle` | Alterna la flag `done` | `200`, `404` |
| DELETE | `/tasks/{id}` | Elimina una tarea | `204`, `404` |

## Flujo de la petición

```mermaid
flowchart LR
    Client([Cliente]) -->|petición HTTP| Endpoint[Endpoint Minimal API]
    Endpoint --> Validation{¿Entrada válida?}
    Validation -->|No| BadRequest[Respuesta 400 / 404]
    Validation -->|Sí| Repo[ITaskRepository]
    Repo --> EF[(EF Core InMemory)]
    EF --> Repo
    Repo --> Response[Resultado serializado]
    Response -->|respuesta HTTP| Client
```

## Uso

**Requisito:** [.NET SDK 10](https://dotnet.microsoft.com/download)

```bash
dotnet run --project src/TasksApi
```

API en `http://localhost:5080`, Swagger UI en `http://localhost:5080/swagger`.

**Ejemplos:**
```bash
# Crear (201 Created)
curl -i -X POST http://localhost:5080/tasks \
  -H "Content-Type: application/json" \
  -d '{ "title": "Buy milk" }'

# Alternar completado
curl -i -X PATCH http://localhost:5080/tasks/{id}/toggle

# Listar solo las completadas
curl "http://localhost:5080/tasks?done=true"
```

**Pruebas:**
```bash
dotnet test
```
xUnit + `Microsoft.AspNetCore.Mvc.Testing` ejercitando la API de punta a punta: creación, búsqueda, listado, filtro `done`, actualización, toggle, eliminación y validación.

**Estructura:**
```
tasks-api-dotnet/
├── src/TasksApi/            # Minimal API
│   ├── Data/                # DbContext EF Core
│   ├── Endpoints/           # Mapeo de endpoints
│   ├── Models/               # Entidad y DTOs
│   └── Repositories/         # ITaskRepository + implementación EF
└── tests/TasksApi.Tests/     # Pruebas de integración xUnit
```

## Licencia

[MIT](LICENSE).

<div align="center">
  <img src="https://file.loading.io/color/feature/thumb/Blues-8.png?" width="100%" height="10px" alt="divider"/>
</div>

<p align="center"><sub>Desarrollado por <strong><a href="https://github.com/geoggrigori">Grigori</a></strong> · 2026</sub></p>
