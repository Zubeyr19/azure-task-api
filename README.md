# Azure Task API

A small, production-style REST API for managing tasks, built with ASP.NET Core 8. It is designed to run locally on SQLite with zero setup, and on Azure SQL in the cloud, deployed to Azure App Service through a GitHub Actions pipeline.

## Tech stack

- ASP.NET Core 8 (Web API, controllers)
- Entity Framework Core 8 (SQLite locally, Azure SQL in the cloud)
- xUnit, Moq and EF Core InMemory for testing
- Swagger / OpenAPI
- Docker and Docker Compose
- GitHub Actions for CI/CD

## Features

- Full CRUD for tasks with a clean controller / service / data layering
- Status workflow on each task (`Todo`, `InProgress`, `Done`)
- Database provider chosen automatically from the connection string, SQLite for local development and SQL Server / Azure SQL in production
- Structured logging on the API endpoints
- Swagger UI in development
- Unit tests covering both the service layer and the controller

## API

| Method | Route | Description |
| --- | --- | --- |
| GET | `/api/tasks` | List all tasks |
| GET | `/api/tasks/{id}` | Get a single task |
| POST | `/api/tasks` | Create a task |
| PUT | `/api/tasks/{id}` | Update a task |
| DELETE | `/api/tasks/{id}` | Delete a task |

## Running locally

```bash
cd src/AzureTaskApi
dotnet run
```

The API starts on the port shown in the console and uses a local SQLite file (`tasks.db`) by default. Swagger UI is available at `/swagger` in development.

To run against SQL Server instead, set a full connection string:

```bash
export ConnectionStrings__DefaultConnection="Server=...;Database=tasks;User Id=...;Password=...;"
```

## Running with Docker

```bash
docker compose up --build
```

This starts the API together with a SQL Server 2022 container.

## Running the tests

```bash
dotnet test
```

The test suite uses EF Core InMemory and Moq, so it runs without a database.

## Project structure

```
src/AzureTaskApi      ASP.NET Core API (Controllers, Services, Data, Models)
tests/AzureTaskApi.Tests   xUnit tests for the service and controller layers
.github/workflows     GitHub Actions CI/CD pipeline
```
