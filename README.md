# ToDoList API

A REST API for task management (To-Do List) with JWT authentication and PostgreSQL.

## Features

- ✅ JWT Authentication (Register/Login)
- ✅ Complete CRUD operations for tasks
- ✅ Task status (Pending, In Progress, Completed)
- ✅ Pagination
- ✅ Status filters
- ✅ Task statistics
- ✅ Data validation
- ✅ Swagger documentation

## Technologies Used

- **ASP.NET Core 8.0**
- **Entity Framework Core**
- **PostgreSQL** with Npgsql
- **JWT Bearer Authentication**
- **BCrypt** for password hashing
- **Swagger/OpenAPI** for documentation

## Setup Instructions

### 1. Clone the Repository

```bash
git clone <repository-url>
cd ToDoList
```

### 2. Configure Database

Make sure PostgreSQL is installed and running. Create databases:

```sql
CREATE DATABASE todolist_db;
CREATE DATABASE todolist_dev_db; -- For development
```

### 3. Configure Connection Strings

Copy the template files and update with your credentials:

```bash
# Copy configuration templates
cp ToDoList/appsettings.template.json ToDoList/appsettings.json
cp ToDoList/appsettings.Development.template.json ToDoList/appsettings.Development.json
```

Then update the connection strings and JWT keys in both files:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=todolist_db;Username=your_username;Password=your_password;Port=5432"
  },
  "Jwt": {
    "Key": "your-super-secret-jwt-key-here-must-be-at-least-32-characters",
    "Issuer": "ToDoListAPI",
    "Audience": "ToDoListClient"
  }
}
```

### 4. Run Migrations

```bash
cd ToDoList
dotnet ef migrations add InitialCreate
dotnet ef database update
```

### 5. Run the Application

```bash
dotnet restore
dotnet run
```

The API will be available at `https://localhost:7021` or the configured port.

## API Documentation

Access the Swagger documentation at: `https://localhost:7021/swagger`

## API Endpoints

### Authentication

| Method | Endpoint             | Description       |
| ------ | -------------------- | ----------------- |
| POST   | `/api/auth/register` | Register new user |
| POST   | `/api/auth/login`    | User login        |

### Tasks (Requires Authentication)

| Method | Endpoint                 | Description                              |
| ------ | ------------------------ | ---------------------------------------- |
| GET    | `/api/todos`             | List tasks (with pagination and filters) |
| GET    | `/api/todos/{id}`        | Get task by ID                           |
| POST   | `/api/todos`             | Create new task                          |
| PUT    | `/api/todos/{id}`        | Update task                              |
| PATCH  | `/api/todos/{id}/status` | Update status only                       |
| DELETE | `/api/todos/{id}`        | Delete task                              |
| GET    | `/api/todos/stats`       | Get statistics                           |

### Task Status Values

- `0` - Pending
- `1` - In Progress
- `2` - Completed

## Security Best Practices

### Environment Variables (Recommended for Production)

Instead of storing sensitive data in configuration files, use environment variables:

```bash
export ConnectionStrings__DefaultConnection="Host=prod-server;Database=todolist_prod;Username=prod_user;Password=secure_password;Port=5432"
export Jwt__Key="your-production-jwt-key-at-least-32-characters-long"
```

### Configuration Files

- **Never commit real credentials** to version control
- Use template files for reference
- Different configurations per environment
- Strong JWT keys (32+ characters)

## Database Monitoring

Use DBeaver or similar tools to monitor the database:

1. Connect to PostgreSQL
2. Use credentials from your configuration
3. View `Users` and `TodoItems` tables

## Project Structure

```
ToDoList/
├── Controllers/          # API Controllers
├── Data/                # DbContext
├── DTOs/                # Data Transfer Objects
├── Models/              # Data models
├── Services/            # Business logic
├── Program.cs           # Application configuration
└── appsettings.json     # Configuration settings
```

## Production Deployment

For production deployment, ensure:

1. Use environment variables for sensitive data
2. Enable HTTPS
3. Configure CORS appropriately
4. Use strong JWT keys
5. Set up proper logging
6. Use a production database server

## Example Usage

### 1. Register User

```bash
curl -X POST "https://localhost:7021/api/auth/register" \
  -H "Content-Type: application/json" \
  -d '{
    "name": "John Doe",
    "email": "john@example.com",
    "password": "123456"
  }'
```

### 2. Login

```bash
curl -X POST "https://localhost:7021/api/auth/login" \
  -H "Content-Type: application/json" \
  -d '{
    "email": "john@example.com",
    "password": "123456"
  }'
```

### 3. Create Task (with token)

```bash
curl -X POST "https://localhost:7021/api/todos" \
  -H "Content-Type: application/json" \
  -H "Authorization: Bearer YOUR_JWT_TOKEN" \
  -d '{
    "title": "Learn ASP.NET Core",
    "description": "Study Web APIs and Entity Framework",
    "dueDate": "2025-08-10T10:00:00Z"
  }'
```

## Contributing

1. Fork the repository
2. Create a feature branch
3. Make your changes
4. Add tests if applicable
5. Submit a pull request

## License

This project is licensed under the MIT License - see the LICENSE file for details.
