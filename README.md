<!-- ══════════════════════════ TÍTULO ══════════════════════════ -->
<div align="center">
  <img src="docs/title-banner.svg" width="100%" alt="tasks-api-dotnet"/>
</div>

<br/>

<!-- ══════════════════════ IDIOMAS / LANGUAGES ══════════════════════ -->
<div align="center">
<a href="README.md"><img src="https://img.shields.io/badge/Português-1987F0?style=for-the-badge" alt="Português"/></a>
<a href="README.en.md"><img src="https://img.shields.io/badge/English-555555?style=for-the-badge" alt="English"/></a>
<a href="README.es.md"><img src="https://img.shields.io/badge/Español-555555?style=for-the-badge" alt="Español"/></a>
</div>

<br/>

<h1 align="center">tasks-api-dotnet</h1>
<p align="center"><em>Minimal API .NET limpa e bem testada para gerenciamento de tarefas</em></p>
<p align="center"><strong>ASP.NET Core Minimal API → EF Core InMemory → xUnit</strong></p>

<div align="center">
<a href="https://github.com/geoggrigori/tasks-api-dotnet/actions/workflows/ci.yml"><img src="https://github.com/geoggrigori/tasks-api-dotnet/actions/workflows/ci.yml/badge.svg" alt="CI"/></a>
<img src="https://img.shields.io/badge/.NET_10-512BD4?style=flat-square&logo=dotnet&logoColor=white" alt="dotnet"/>
<img src="https://img.shields.io/badge/C%23_12-239120?style=flat-square&logo=csharp&logoColor=white" alt="csharp"/>
<img src="https://img.shields.io/badge/Swagger-85EA2D?style=flat-square&logo=swagger&logoColor=black" alt="swagger"/>
<img src="https://img.shields.io/badge/License-MIT-2E7D32?style=flat-square" alt="license"/>
</div>

<div align="center">
<a href="#sobre"><img src="https://img.shields.io/badge/▸_SOBRE-1987F0?style=for-the-badge" alt="sobre"/></a>
<a href="#endpoints"><img src="https://img.shields.io/badge/▸_ENDPOINTS-000000?style=for-the-badge" alt="endpoints"/></a>
<a href="#fluxo-da-requisição"><img src="https://img.shields.io/badge/▸_FLUXO-1987F0?style=for-the-badge" alt="fluxo"/></a>
<a href="#uso"><img src="https://img.shields.io/badge/▸_USO-000000?style=for-the-badge" alt="uso"/></a>
</div>

<br/>

> 📘 **Swagger UI incluso** — rode a API e abra `/swagger` pra explorar tudo interativamente.

## Sobre

Uma **Minimal API** limpa e bem testada para gerenciamento de tarefas, construída com **ASP.NET Core**, **EF Core** e **xUnit**.

**Destaques:**
- CRUD completo de tarefas via interface REST.
- Filtro opcional `?done=true|false` ao listar.
- Validação de entrada: títulos vazios são rejeitados com `400`.
- Semântica REST correta: `201 Created` com header `Location`, `204 No Content` no delete, `404 Not Found` pra ids desconhecidos.
- Abstração de repositório (`ITaskRepository`) sobre o provider EF Core InMemory.
- Documentação interativa **Swagger UI** / OpenAPI.
- Testes de integração com `WebApplicationFactory` cobrindo todo endpoint.

## Endpoints

| Método | Rota | Descrição | Status |
|---|---|---|---|
| GET | `/tasks` | Lista tarefas (opcional `?done=true\|false`) | `200` |
| GET | `/tasks/{id}` | Busca uma tarefa por id | `200`, `404` |
| POST | `/tasks` | Cria uma nova tarefa | `201`, `400` |
| PUT | `/tasks/{id}` | Substitui uma tarefa existente | `200`, `400`, `404` |
| PATCH | `/tasks/{id}/toggle` | Alterna a flag `done` | `200`, `404` |
| DELETE | `/tasks/{id}` | Remove uma tarefa | `204`, `404` |

## Fluxo da Requisição

```mermaid
flowchart LR
    Client([Cliente]) -->|requisição HTTP| Endpoint[Endpoint Minimal API]
    Endpoint --> Validation{Entrada válida?}
    Validation -->|Não| BadRequest[Resposta 400 / 404]
    Validation -->|Sim| Repo[ITaskRepository]
    Repo --> EF[(EF Core InMemory)]
    EF --> Repo
    Repo --> Response[Resultado serializado]
    Response -->|resposta HTTP| Client
```

## Uso

**Pré-requisito:** [.NET SDK 10](https://dotnet.microsoft.com/download)

```bash
dotnet run --project src/TasksApi
```

API em `http://localhost:5080`, Swagger UI em `http://localhost:5080/swagger`.

**Exemplos:**
```bash
# Criar (201 Created)
curl -i -X POST http://localhost:5080/tasks \
  -H "Content-Type: application/json" \
  -d '{ "title": "Buy milk" }'

# Alternar conclusão
curl -i -X PATCH http://localhost:5080/tasks/{id}/toggle

# Listar só as concluídas
curl "http://localhost:5080/tasks?done=true"
```

**Testes:**
```bash
dotnet test
```
xUnit + `Microsoft.AspNetCore.Mvc.Testing` exercitando a API ponta a ponta: criação, busca, listagem, filtro `done`, atualização, toggle, exclusão e validação.

**Estrutura:**
```
tasks-api-dotnet/
├── src/TasksApi/            # Minimal API
│   ├── Data/                # DbContext EF Core
│   ├── Endpoints/           # Mapeamento de endpoints
│   ├── Models/               # Entidade e DTOs
│   └── Repositories/         # ITaskRepository + implementação EF
└── tests/TasksApi.Tests/     # Testes de integração xUnit
```

## Licença

[MIT](LICENSE).

<div align="center">
  <img src="https://file.loading.io/color/feature/thumb/Blues-8.png?" width="100%" height="10px" alt="divider"/>
</div>

<p align="center"><sub>Desenvolvido por <strong><a href="https://github.com/geoggrigori">Grigori</a></strong> · 2026</sub></p>
