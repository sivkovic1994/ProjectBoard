# ProjectBoard - Microservices Demo

.NET 8 microservices application with JWT authentication and Docker support.

## Architecture
- **AuthService** - User registration, login, JWT token generation
- **TaskService** - CRUD operations for tasks (requires JWT authentication)

## Tech Stack
- .NET 8 Web API
- Entity Framework Core + SQLite
- JWT Authentication
- Docker & Docker Compose
- Swagger/OpenAPI

## 🚀 Run with Docker
docker-compose up --build

Access services: AuthService http://localhost:5162/swagger | TaskService http://localhost:5279/swagger

Stop services:
docker-compose down

Reset databases:
docker-compose down -v

## 🛠️ Run from Visual Studio
1. Set Multiple startup projects (AuthService + TaskService)
2. Press F5
3. Access: AuthService https://localhost:7078/swagger | TaskService https://localhost:7088/swagger

## 🧪 Testing Flow

**1. Register user (AuthService)**
POST http://localhost:5162/api/auth/register {"username": "testuser", "email": "test@example.com", "password": "Test123456"}

**2. Login (AuthService)**
POST http://localhost:5162/api/auth/login {"email": "test@example.com", "password": "Test123456"}

Copy JWT token from response.

**3. Authorize in TaskService**
Open http://localhost:5279/swagger → Click Authorize → Enter `{your_token}`

**4. Create task (TaskService)**
POST http://localhost:5279/api/task {"title": "Task 1", "description": "This is task 1"}

**5. Get all tasks (TaskService)**
GET http://localhost:5279/api/task

## 📝 API Endpoints

**AuthService (port 5162)**
- POST /api/auth/register - Register new user
- POST /api/auth/login - Login and get JWT token
- GET /api/auth/me - Get current user info (protected)

**TaskService (port 5279)**
- POST /api/task - Create task (protected)
- GET /api/task - Get all user tasks (protected)
- PUT /api/task/{id} - Update task (protected)
- DELETE /api/task/{id} - Delete task (protected)