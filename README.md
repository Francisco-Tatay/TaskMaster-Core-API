# TaskManagerPro

TaskManagerPro is an ASP.NET Core Web API for managing user tasks with a layered architecture (API -> Application -> Domain -> Infrastructure), MySQL/MariaDB persistence through Entity Framework Core, and JWT authentication with refresh tokens.

## Features

- Register, login, logout and refresh-token flows (JWT + BCrypt password hashing)
- Create tasks for a user
- Get all tasks by user ID
- Update existing tasks
- Delete tasks by ID
- Repository + service structure for separation of concerns
- EF Core migrations included in the repository
- OpenAPI endpoint enabled in Development
- Unit tests (xUnit + NSubstitute + Shouldly) for the application services

## Tech Stack

- .NET `10.0` (`net10.0`)
- ASP.NET Core Web API
- Entity Framework Core `9.0.0`
- Pomelo MySQL provider (`Pomelo.EntityFrameworkCore.MySql`)
- MariaDB/MySQL backend
- JWT Bearer authentication (`Microsoft.AspNetCore.Authentication.JwtBearer`)
- BCrypt password hashing (`BCrypt.Net-Next`)
- Testing: xUnit, NSubstitute, Shouldly

## Project Structure

```text
TaskManagerPro/
|- Program.cs
|- appsettings.json
|- Migrations/
|- src/
|  |- API/                       # Controllers (AuthController, TaskController)
|  |- Application/               # Services, DTOs, common interfaces
|  |  |- Common/                 # Interfaces (ITokenService, IPasswordHasher, IIdGenerator) + PageResponse
|  |  |- DTOs/                   # Auth and Tasks request/response records
|  |  |- Services/               # AuthService, TaskServices
|  |- Domain/                    # Entities (Task, User, RefreshToken)
|  |  |- Interfaces/             # ITaskRepository, IUserRepository, IRefreshTokenRepository
|  |- Infrastructure/            # EF Core AppDbContext, repositories, JWT/BCrypt implementations
|- test/
|  |- TestTaskManager/           # xUnit test project
|     |- TaskServiceTest.cs      # Tests for TaskServices
|     |- AuthServiceTest.cs      # Tests for AuthService
```

> Note: the project is a single `.csproj` (`TaskManagerPro.csproj`) with logical folders under `src/`, plus a separate test project under `test/`.

## Prerequisites

- .NET SDK 10
- MariaDB/MySQL server
- Optional: `dotnet-ef` CLI tool

Install EF CLI if needed:

```bash
dotnet tool install --global dotnet-ef
```

## Configuration

The API reads the DB connection string from `ConnectionStrings:DefaultConnection`.

Current source file (`appsettings.json`) includes a concrete connection string. For safer local/team setups, override via environment variable instead of committing secrets:

```bash
export ConnectionStrings__DefaultConnection="Server=localhost;Port=3306;Database=Phoenix_Tasks;Uid=root;Pwd=your_password;"
```

JWT settings (`Jwt:Key`, `Jwt:Issuer`, `Jwt:Audience`) also come from `appsettings.json` — replace the placeholder key with a real secret (or use environment variables / user secrets).

> Note: `Program.cs` currently sets a MariaDB server version (`12.2.2`) in `UseMySql(...)`. If your DB version differs, update that value.

## Run Locally

```bash
dotnet restore
dotnet build
dotnet run --project TaskManagerPro.csproj
```

By default (Development profile), the API runs on:

- `http://localhost:5179`
- `https://localhost:7087`

OpenAPI is mapped in Development. Try:

- `http://localhost:5179/openapi/v1.json`

## Database Migrations

Apply existing migrations:

```bash
dotnet ef database update
```

Create a new migration:

```bash
dotnet ef migrations add <MigrationName>
```

## API Endpoints

### Auth

Base route: `/api/Auth`

| Method | Route | Body | Description |
|---|---|---|---|
| `POST` | `/api/Auth/register` | `{ "email", "password", "passwordverfication" }` | Register a new user; returns access + refresh tokens |
| `POST` | `/api/Auth/login` | `{ "email", "password" }` | Login; returns access + refresh tokens |
| `POST` | `/api/Auth/logout` | `{ "refreshToken" }` | Revokes the refresh token |
| `POST` | `/api/Auth/refresh-token` | `{ "refreshToken" }` | Exchanges a valid refresh token for new tokens |

### Tasks

Base route: `/taskManagerPro/Task` (IDs are GUIDs)

| Method | Route | Body | Description |
|---|---|---|---|
| `GET` | `/taskManagerPro/Task/{userId}` | — | Get all tasks for a user |
| `POST` | `/taskManagerPro/Task/{userId}` | `{ "title", "description" }` | Create a task for a user |
| `PUT` | `/taskManagerPro/Task/{taskId}` | `{ "id", "title", "description" }` | Update a task (body `id` must match the URL) |
| `DELETE` | `/taskManagerPro/Task/{taskId}` | — | Delete a task |

Example:

```bash
curl -X POST "http://localhost:5179/taskManagerPro/Task/00000000-0000-0000-0000-000000000001" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Learn Hexagonal Architecture",
    "description": "Finish backend module"
  }'
```

## Testing

The test project lives in `test/TestTaskManager/` (xUnit + NSubstitute + Shouldly). Run all tests:

```bash
dotnet test
```

Run only a specific test by name:

```bash
dotnet test --filter "FullyQualifiedName~Login"
```

Current coverage:

- `TaskServiceTest.cs` — 6 tests: list tasks (with data / empty), create, update (exists / not found), delete
- `AuthServiceTest.cs` — tests for register, login and logout flows

## Development Notes

- `TaskServices.CreateTaskAsync(...)` forces `IsCompleted = false` at creation time.
- `TaskManagerPro.http` contains ready-to-run HTTP requests for local testing.
- The `test/` folder is excluded from the main project compilation via `<Compile Remove="test\**"/>` in `TaskManagerPro.csproj`.

## Docker

A `Dockerfile` is included, but it currently references multiple `.csproj` files under `src/` that are not present in this repository snapshot. If you want container builds, align the `Dockerfile` with the current single-project layout (`TaskManagerPro.csproj`) first.

## Suggested Next Improvements

- Expose task-by-id endpoint (`GET /taskManagerPro/Task/{taskId}`)
- Add DTO validation attributes (`[Required]`, `[EmailAddress]`, ...)
- Enforce `[Authorize]` on task endpoints
- Finish `AuthService` tests (refresh-token flows) and add controller tests
- Add integration tests with a real (containerized) database
- Move all secrets to environment variables / user secrets
- Remove dead code (`CreateTaskDto` interface is unused)
- Fix the Dockerfile and the stale README-referenced project layout