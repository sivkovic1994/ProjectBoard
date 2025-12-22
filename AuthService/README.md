# AuthService (.NET 8)

Authentication microservice built with ASP.NET Core.

## Features
- User registration
- User login with JWT
- Protected endpoints
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
https://localhost:5001/swagger

## Endpoints
- POST /api/auth/register
- POST /api/auth/login
- GET /api/auth/me