# TaskManagerPro

TaskManagerPro is an ASP.NET Core Web API for managing user tasks with a layered architecture (API -> Application -> Domain -> Infrastructure) and MySQL/MariaDB persistence through Entity Framework Core.

## Features

- Create tasks for a user
- Get all tasks by user ID
- Update existing tasks
- Delete tasks by ID
- Repository + service structure for separation of concerns
- EF Core migrations included in the repository
- OpenAPI endpoint enabled in Development

## Tech Stack

- .NET `10.0` (`net10.0`)
- ASP.NET Core Web API
- Entity Framework Core `9.0.0`
- Pomelo MySQL provider (`Pomelo.EntityFrameworkCore.MySql`)
- MariaDB/MySQL backend

## Project Structure

```text
TaskManagerPro/
|- Program.cs
|- appsettings.json
|- Migrations/
|- src/
|  |- TaskMasterPro.API/                # Controllers
|  |- TaskMasterPro.Application/        # Services / use cases
|  |- TaskMasterPro.Domain/             # Entities
|  |- TaskMasterPro.Infrastructure/     # DbContext + repositories
|  |- TaskManagerPro.Interfaces/        # Repository interfaces
|- test/                                # Test project folders (currently empty)
```

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

Base route: `/taskManagerPro/Task`

### Get tasks by user ID

- `GET /taskManagerPro/Task/{userId}`

Example:

```bash
curl "http://localhost:5179/taskManagerPro/Task/1"
```

### Create task

- `POST /taskManagerPro/Task`

Example:

```bash
curl -X POST "http://localhost:5179/taskManagerPro/Task" \
  -H "Content-Type: application/json" \
  -d '{
    "title": "Learn Hexagonal Architecture",
    "description": "Finish backend module",
    "userId": 1
  }'
```

### Update task

- `PUT /taskManagerPro/Task/{taskId}`

Example:

```bash
curl -X PUT "http://localhost:5179/taskManagerPro/Task/1" \
  -H "Content-Type: application/json" \
  -d '{
    "id": 1,
    "title": "Updated title",
    "description": "Updated description",
    "isCompleted": true,
    "userId": 1
  }'
```

### Delete task

- `DELETE /taskManagerPro/Task/{taskId}`

Example:

```bash
curl -X DELETE "http://localhost:5179/taskManagerPro/Task/1"
```

## Development Notes

- `TaskServices.CreateTaskAsync(...)` forces `IsCompleted = false` at creation time.
- `DeleteTaskAsync(...)` is exposed through `DELETE /taskManagerPro/Task/{taskId}`.
- `TaskManagerPro.http` contains ready-to-run HTTP requests for local testing.

## Docker

A `Dockerfile` is included, but it currently references multiple `.csproj` files under `src/` that are not present in this repository snapshot. If you want container builds, align the `Dockerfile` with the current single-project layout (`TaskManagerPro.csproj`) first.

## Testing

The `test/` folders are present as placeholders, but no automated test files are currently committed.

You can still run test discovery safely:

```bash
dotnet test
```

## Suggested Next Improvements

- Add authentication/authorization flow (JWT is referenced in dependencies)
- Expose task-by-id endpoint
- Add DTOs + validation attributes for request models
- Add integration and unit tests in `test/`
- Move all secrets to environment variables / user secrets

