# TaskService (.NET 8)

Task management microservice built with ASP.NET Core.

## Features
- Create tasks
- Retrieve user tasks
- Delete tasks
- JWT-protected endpoints
- EF Core + SQLite
- Code-first migrations

## Tech Stack
- ASP.NET Core
- Entity Framework Core
- SQLite
- JWT Authentication

## Run locally
1. Update database:
2. Run the app
3. Open Swagger at:
https://localhost:7088/swagger

## Endpoints
- POST /api/task - Create a new task
- GET /api/task - Get all tasks for authenticated user
- DELETE /api/task/{id} - Delete a specific task