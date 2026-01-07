# NexusAdmin Backend

> Serverless admin panel built with Azure Functions, .NET 9, and Hexagonal Architecture

[![.NET](https://img.shields.io/badge/.NET-9.0-512BD4?logo=dotnet)](https://dotnet.microsoft.com/)
[![Azure Functions](https://img.shields.io/badge/Azure%20Functions-v4-0062AD?logo=azurefunctions)](https://azure.microsoft.com/en-us/services/functions/)
[![MongoDB](https://img.shields.io/badge/MongoDB-7.0-47A248?logo=mongodb)](https://www.mongodb.com/)
[![License](https://img.shields.io/badge/license-MIT-blue.svg)](LICENSE)

## 📋 Table of Contents

- [Overview](#overview)
- [Architecture](#architecture)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Prerequisites](#prerequisites)
- [Getting Started](#getting-started)
- [Project Structure](#project-structure)
- [API Documentation](#api-documentation)
- [Configuration](#configuration)
- [Development](#development)
- [Deployment](#deployment)
- [Contributing](#contributing)

## 🎯 Overview

NexusAdmin is a modern, serverless admin panel backend built with Azure Functions and following **Hexagonal Architecture** (Ports & Adapters) principles. It provides a scalable, maintainable, and cloud-native solution for user management and administration.

### Key Highlights

- **🏗️ Hexagonal Architecture**: Clean separation between business logic and infrastructure
- **☁️ Serverless**: Pay-per-execution with Azure Functions
- **🔄 CQRS-lite**: Separated use cases for better organization
- **📦 Domain-Driven Design**: Rich domain models with business rules
- **🐳 Docker**: Local development environment with Docker Compose
- **🔒 Type-Safe**: Strong typing with C# and value objects

## 🏛️ Architecture

This project follows **Hexagonal Architecture** (also known as Ports & Adapters):

```
┌─────────────────────────────────────────────────────────┐
│                    Azure Functions                       │
│                    (HTTP Triggers)                       │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                      Use Cases                           │
│          (Application Business Logic)                    │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                    Core Domain                           │
│        (Entities, Value Objects, Interfaces)             │
└────────────────────┬────────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────────┐
│                  Infrastructure                          │
│         (MongoDB, Email, External Services)              │
└─────────────────────────────────────────────────────────┘
```

### Benefits

- ✅ **Testable**: Business logic has zero infrastructure dependencies
- ✅ **Flexible**: Swap MongoDB for SQL Server without touching business rules
- ✅ **Maintainable**: Clear boundaries and single responsibility
- ✅ **Portable**: Core domain can be reused in any framework

## ✨ Features

### User Management

- ✅ Create users with validation
- ✅ Update user information
- ✅ Activate/Deactivate users
- ✅ Delete users
- ✅ List users with pagination
- ✅ Role-based access (User, Manager, Admin)
- ✅ Email notifications (welcome emails)

### Technical Features

- ✅ Input validation with value objects
- ✅ Rich domain models with business rules
- ✅ Structured error handling
- ✅ Comprehensive logging
- ✅ Email service integration (SMTP)
- ✅ MongoDB persistence with proper indexes
- ✅ RESTful API design

## 🛠️ Tech Stack

### Core
- **.NET 9** - Latest .NET framework
- **C# 12** - Modern C# features

### Backend
- **Azure Functions v4** - Serverless compute
- **Isolated Worker Process** - Better performance and flexibility

### Database
- **MongoDB 7.0** - NoSQL database
- **MongoDB.Driver** - Official .NET driver

### Email
- **MailKit** - SMTP email sending
- **MailHog** - Email testing (development)

### Development Tools
- **Docker & Docker Compose** - Containerization
- **Azurite** - Azure Storage Emulator
- **Application Insights** - Telemetry and monitoring

## 📦 Prerequisites

Before you begin, ensure you have the following installed:

- [.NET 9 SDK](https://dotnet.microsoft.com/download) (9.0 or later)
- [Azure Functions Core Tools](https://docs.microsoft.com/en-us/azure/azure-functions/functions-run-local) (v4)
- [Docker Desktop](https://www.docker.com/products/docker-desktop) (for local services)
- [Git](https://git-scm.com/)

### Optional but Recommended
- [Visual Studio 2022](https://visualstudio.microsoft.com/) or [VS Code](https://code.visualstudio.com/)
- [Postman](https://www.postman.com/) or similar API testing tool
- [MongoDB Compass](https://www.mongodb.com/products/compass) (GUI for MongoDB)

## 🚀 Getting Started

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/NexusAdminBackend.git
cd NexusAdminBackend
```

### 2. Start Infrastructure Services

Start MongoDB, MailHog, and other services with Docker Compose:

```bash
docker-compose up -d
```

**Services started:**
- MongoDB: `localhost:27017`
- Mongo Express (UI): `http://localhost:8081` (admin/admin)
- MailHog (Email testing): `http://localhost:8025`
- Redis: `localhost:6379`
- PostgreSQL: `localhost:5432` (optional)

### 3. Restore Dependencies

```bash
dotnet restore
```

### 4. Build the Solution

```bash
dotnet build
```

### 5. Configure Settings

Update `src/NexusAdmin.Functions/local.settings.json`:

```json
{
  "IsEncrypted": false,
  "Values": {
    "AzureWebJobsStorage": "UseDevelopmentStorage=true",
    "FUNCTIONS_WORKER_RUNTIME": "dotnet-isolated",
    
    "MongoDB__ConnectionString": "mongodb://admin:password123@localhost:27017",
    "MongoDB__DatabaseName": "NexusAdminDB",
    "MongoDB__UsersCollectionName": "Users",
    
    "Email__SmtpHost": "localhost",
    "Email__SmtpPort": "1025",
    "Email__FromEmail": "noreply@nexusadmin.com",
    "Email__FromName": "NexusAdmin",
    "Email__UseSsl": "false"
  }
}
```

### 6. Run the Application

```bash
cd src/NexusAdmin.Functions
func start
```

The API will be available at: `http://localhost:7071`

### 7. Test the API

```bash
# Create a user
curl -X POST http://localhost:7071/api/users \
  -H "Content-Type: application/json" \
  -d '{
    "email": "test@example.com",
    "name": "Test User",
    "role": "User"
  }'

# Get all users
curl http://localhost:7071/api/users

# Get specific user
curl http://localhost:7071/api/users/{userId}
```

## 📁 Project Structure

```
NexusAdminBackend/
├── src/
│   ├── NexusAdmin.Core/                    # Domain Layer
│   │   ├── Entities/                       # Domain entities
│   │   │   └── User.cs
│   │   ├── ValueObjects/                   # Value objects
│   │   │   └── Email.cs
│   │   ├── Interfaces/                     # Ports (abstractions)
│   │   │   ├── Repositories/
│   │   │   │   └── IUserRepository.cs
│   │   │   └── Services/
│   │   │       └── IEmailService.cs
│   │   ├── UseCases/                       # Application use cases
│   │   │   └── Users/
│   │   │       ├── CreateUser/
│   │   │       ├── GetUser/
│   │   │       ├── UpdateUser/
│   │   │       ├── DeleteUser/
│   │   │       ├── ListUsers/
│   │   │       ├── ActivateUser/
│   │   │       └── DeactivateUser/
│   │   └── Exceptions/                     # Domain exceptions
│   │
│   ├── NexusAdmin.Infrastructure/          # Infrastructure Layer
│   │   ├── Persistence/
│   │   │   └── MongoDB/
│   │   │       ├── Configuration/
│   │   │       │   ├── MongoDbSettings.cs
│   │   │       │   └── MongoDbContext.cs
│   │   │       └── Repositories/
│   │   │           ├── UserDocument.cs
│   │   │           └── MongoDbUserRepository.cs
│   │   ├── Services/
│   │   │   └── Email/
│   │   │       ├── EmailSettings.cs
│   │   │       └── SmtpEmailService.cs
│   │   └── Configuration/
│   │       └── DependencyInjection.cs
│   │
│   └── NexusAdmin.Functions/               # API Layer (Azure Functions)
│       ├── Users/                          # Function endpoints
│       │   ├── CreateUserFunction.cs
│       │   ├── GetUserFunction.cs
│       │   ├── ListUsersFunction.cs
│       │   ├── UpdateUserFunction.cs
│       │   ├── DeleteUserFunction.cs
│       │   ├── ActivateUserFunction.cs
│       │   └── DeactivateUserFunction.cs
│       ├── Configuration/
│       │   └── JsonOptions.cs
│       ├── DTO/                            # Data transfer objects
│       ├── Program.cs                      # Startup configuration
│       ├── host.json                       # Function host settings
│       └── local.settings.json             # Local configuration
│
├── docker-compose.yml                      # Docker services
├── NexusAdminBackend.sln                  # Solution file
└── README.md                               # This file
```

### Layer Responsibilities

| Layer | Responsibility | Dependencies |
|-------|---------------|--------------|
| **Core** | Business logic, domain rules | None (pure domain) |
| **Infrastructure** | Database, email, external APIs | Core |
| **Functions** | HTTP endpoints, serialization | Core, Infrastructure |

## 📚 API Documentation

### Base URL
```
http://localhost:7071/api
```

### Endpoints

#### Users

| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/users` | Create a new user |
| GET | `/users` | List users (paginated) |
| GET | `/users/{id}` | Get user by ID |
| PATCH | `/users/{id}` | Update user |
| DELETE | `/users/{id}` | Delete user |
| POST | `/users/{id}/activate` | Activate user |
| POST | `/users/{id}/deactivate` | Deactivate user |

### Request/Response Examples

#### Create User

**Request:**
```http
POST /api/users
Content-Type: application/json

{
  "email": "john.doe@example.com",
  "name": "John Doe",
  "role": "User"
}
```

**Response (201 Created):**
```json
{
  "success": true,
  "message": "User created successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "name": "John Doe",
    "role": "User",
    "isActive": true,
    "createdAt": "2024-12-28T10:30:00Z"
  }
}
```

#### List Users

**Request:**
```http
GET /api/users?page=1&pageSize=10
```

**Response (200 OK):**
```json
{
  "success": true,
  "data": {
    "users": [
      {
        "id": "550e8400-e29b-41d4-a716-446655440000",
        "email": "john.doe@example.com",
        "name": "John Doe",
        "role": "User",
        "isActive": true,
        "createdAt": "2024-12-28T10:30:00Z",
        "updatedAt": null
      }
    ],
    "page": 1,
    "pageSize": 10,
    "totalCount": 1,
    "totalPages": 1
  }
}
```

#### Update User

**Request:**
```http
PATCH /api/users/550e8400-e29b-41d4-a716-446655440000
Content-Type: application/json

{
  "name": "John Smith",
  "role": "Manager"
}
```

**Response (200 OK):**
```json
{
  "success": true,
  "message": "User updated successfully",
  "data": {
    "id": "550e8400-e29b-41d4-a716-446655440000",
    "email": "john.doe@example.com",
    "name": "John Smith",
    "role": "Manager",
    "isActive": true,
    "updatedAt": "2024-12-28T11:30:00Z"
  }
}
```

### Error Responses

**400 Bad Request:**
```json
{
  "error": "Name must be at least 3 characters long"
}
```

**404 Not Found:**
```json
{
  "error": "User with ID '550e8400...' not found"
}
```

**409 Conflict:**
```json
{
  "error": "A user with email 'john.doe@example.com' already exists"
}
```

**500 Internal Server Error:**
```json
{
  "error": "Internal server error"
}
```

## ⚙️ Configuration

### Environment Variables

Configure these in `local.settings.json` for local development or in Azure Function App settings for production:

| Variable | Description | Example |
|----------|-------------|---------|
| `MongoDB__ConnectionString` | MongoDB connection string | `mongodb://localhost:27017` |
| `MongoDB__DatabaseName` | Database name | `NexusAdminDB` |
| `MongoDB__UsersCollectionName` | Users collection name | `Users` |
| `Email__SmtpHost` | SMTP server host | `localhost` |
| `Email__SmtpPort` | SMTP server port | `1025` |
| `Email__FromEmail` | Sender email address | `noreply@nexusadmin.com` |
| `Email__FromName` | Sender name | `NexusAdmin` |
| `Email__UseSsl` | Use SSL for SMTP | `false` |

### Docker Services

Edit `docker-compose.yml` to change default credentials and ports:

```yaml
services:
  mongodb:
    image: mongo:7.0
    ports:
      - "27017:27017"
    environment:
      MONGO_INITDB_ROOT_USERNAME: admin
      MONGO_INITDB_ROOT_PASSWORD: password123
```

## 🔧 Development

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true
```

### Code Quality

```bash
# Format code
dotnet format

# Analyze code
dotnet build /p:TreatWarningsAsErrors=true
```

### Database Management

**Access MongoDB:**
```bash
# Via Mongo Express (Web UI)
http://localhost:8081

# Via MongoDB Compass
mongodb://admin:password123@localhost:27017

# Via CLI
docker exec -it nexusadmin-mongodb mongosh -u admin -p password123
```

**Reset Database:**
```bash
docker-compose down -v
docker-compose up -d
```

### Email Testing

View sent emails in MailHog:
```
http://localhost:8025
```

All emails sent during development are captured here.

## 🚀 Deployment

### Deploy to Azure

1. **Create Azure Resources:**
```bash
# Create resource group
az group create --name NexusAdminRG --location eastus

# Create storage account
az storage account create --name nexusadminstorage --resource-group NexusAdminRG

# Create function app
az functionapp create \
  --name nexusadmin-api \
  --resource-group NexusAdminRG \
  --consumption-plan-location eastus \
  --runtime dotnet-isolated \
  --functions-version 4
```

2. **Configure Application Settings:**
```bash
az functionapp config appsettings set \
  --name nexusadmin-api \
  --resource-group NexusAdminRG \
  --settings \
    "MongoDB__ConnectionString=<your-mongodb-connection-string>" \
    "MongoDB__DatabaseName=NexusAdminDB"
```

3. **Deploy:**
```bash
cd src/NexusAdmin.Functions
func azure functionapp publish nexusadmin-api
```

### Production Considerations

- Use **Azure Cosmos DB** for MongoDB API (globally distributed)
- Enable **Application Insights** for monitoring
- Configure **Azure Key Vault** for secrets
- Set up **CI/CD** with GitHub Actions or Azure DevOps
- Enable **authentication** (Azure AD, API keys)
- Configure **CORS** for your frontend domain
- Set up **custom domain** and SSL certificate
