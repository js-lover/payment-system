# Payment System API

A comprehensive payment system built with **ASP.NET Core 10** following **Clean Architecture** principles. This API provides user authentication, account management, card handling, and transaction processing capabilities.

---

## 📌 Overview

The Payment System API is a robust backend service designed to manage financial operations including:

- **User Authentication & Authorization** - JWT-based security with role-based access control
- **Customer Management** - Create and manage customer profiles
- **Account Management** - Multi-currency bank accounts with balance tracking
- **Card Management** - Debit/credit card creation and lifecycle management
- **Transaction Processing** - Record and query financial transactions
- **Admin Dashboard** - Administrative oversight and reporting

This project demonstrates enterprise-grade API design with proper separation of concerns, dependency injection, and data persistence.

---

## 🏗️ Architecture

The project follows **Clean Architecture** with four distinct layers:

### Layer Structure

1. **Domain Layer** (`payment-system.Domain`)
   - Core business entities and enums
   - No dependencies on other layers
   - Defines the fundamental domain logic

2. **Application Layer** (`payment-system.Application`)
   - Business logic and use cases
   - DTOs (Data Transfer Objects)
   - Repository interfaces and service interfaces
   - Mapping profiles (AutoMapper)
   - Security utilities

3. **Infrastructure Layer** (`payment-system.Infrastructure`)
   - Database context and configurations
   - Repository implementations
   - Database migrations
   - Security implementations (JWT, password hashing with BCrypt)

4. **API Layer** (`payment-system.Api`)
   - Controllers organized by feature
   - Dependency injection setup
   - Middleware configuration
   - Swagger/OpenAPI documentation

### Key Design Patterns

- **Repository Pattern** - Abstracts data access through interfaces
- **Dependency Injection** - Loose coupling and testability
- **DTO Pattern** - Separates internal entities from external API contracts
- **CQRS-Inspired** - Controllers split into Query and Command operations (where applicable)
- **Service Layer Pattern** - Encapsulates business logic

### Project Structure

```
payment-system/
├── payment-system.Api/                    # ASP.NET Core Web API
│   ├── Controllers/
│   │   ├── Account/                       # Account operations
│   │   ├── Admin/                         # Admin dashboard & management
│   │   ├── Auth/                          # Authentication (Login, Register)
│   │   ├── Card/                          # Card management
│   │   ├── Customer/                      # Customer CRUD operations
│   │   └── Transaction/                   # Transaction queries & creation
│   ├── Extensions/                        # Dependency injection setup
│   ├── Program.cs                         # Application entry point
│   └── appsettings.json                   # Configuration
├── payment-system.Application/            # Application layer
│   ├── DTOs/                              # Data transfer objects
│   ├── Services/
│   │   ├── Interfaces/                    # Service contracts
│   │   └── Implementations/               # Service implementations
│   ├── Repositories/                      # Repository interfaces
│   └── Common/
│       ├── Result.cs                      # Standardized result wrapper
│       ├── Mappings/                      # AutoMapper profiles
│       └── Security/                      # Security utilities
├── payment-system.Domain/                 # Domain layer
│   ├── Entities/                          # Core business entities
│   │   ├── User.cs
│   │   ├── Customer.cs
│   │   ├── Account.cs
│   │   ├── Card.cs
│   │   └── Transaction.cs
│   ├── Enums/                             # Domain enumerations
│   │   ├── UserRole.cs
│   │   ├── CardStatus.cs
│   │   ├── TransactionType.cs
│   │   ├── TransactionStatus.cs
│   │   └── Currency.cs
│   └── Common/
│       └── BaseEntity.cs                  # Base entity with common properties
├── payment-system.Infrastructure/         # Infrastructure layer
│   ├── Persistence/
│   │   ├── Contexts/                      # Database contexts
│   │   └── Configurations/                # Entity configurations
│   ├── Repositories/                      # Repository implementations
│   ├── Security/                          # Security implementations
│   └── Migrations/                        # Database migrations
├── payment-system.Tests/                  # Unit & integration tests
│   └── Services/                          # Test suites
└── payment-system.slnx                    # Solution file
```

---

## ⚙️ Technologies Used

### Core Framework
- **ASP.NET Core 10** - Modern web framework for building APIs
- **.NET 10.0** - Latest .NET runtime

### Data Access & ORM
- **Entity Framework Core 10.0.3** - ORM for database operations
- **SQLite 10.0.3** - Default database (development)
- **SQL Server 10.0.3** - Alternative database provider

### Authentication & Security
- **JWT Bearer Authentication** - Token-based API security
- **BCrypt.Net-Next 4.1.0** - Secure password hashing
- **System.IdentityModel.Tokens.Jwt 8.17.0** - JWT token generation and validation

### API Documentation
- **Swashbuckle.AspNetCore 7.0.0** - Swagger/OpenAPI integration for interactive API docs

### Object Mapping
- **AutoMapper** - Object-to-object mapping for DTOs

### Testing
- **xUnit 2.6.2** - Unit testing framework
- **Moq 4.18.4** - Mocking library for unit tests
- **Microsoft.NET.Test.Sdk 17.8.0** - Test SDK

### Additional Libraries
- **Microsoft.EntityFrameworkCore.Proxies** - Lazy loading support
- **Microsoft.EntityFrameworkCore.Tools** - EF Core CLI tools

---

## 🚀 Getting Started

### Prerequisites

- **.NET 10.0 SDK** - Download from [microsoft.com/net](https://microsoft.com/net)
- **Git** - Version control system

### Installation

1. **Clone the repository**
   ```bash
   git clone https://github.com/js-lover/payment-system.git
   cd payment-system
   ```

2. **Restore dependencies**
   ```bash
   dotnet restore
   ```

3. **Apply database migrations** (creates SQLite database)
   ```bash
   cd payment-system.Infrastructure
   dotnet ef database update --project ../payment-system.Infrastructure --startup-project ../payment-system.Api
   ```

4. **Build the solution**
   ```bash
   dotnet build
   ```

### Running the Project

#### Development Mode
```bash
cd payment-system.Api
dotnet run
```

The API will start at:
- **HTTP**: `http://localhost:5000`
- **HTTPS**: `https://localhost:5001`
- **Swagger UI**: `http://localhost:5000/` (when using development profile)

#### Using dotnet CLI
```bash
dotnet run --project payment-system.Api
```

#### Using Visual Studio Code
1. Press `F5` or select **Run > Start Debugging**
2. Select **.NET 5+ and .NET Core** as the environment

---

## 🔧 Configuration

### appsettings.json

The application configuration is managed through `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Data Source=PaymentSystem.db"
  },
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning"
    }
  },
  "JwtSettings": {
    "Secret": "your-secret-key-minimum-32-characters",
    "Issuer": "PaymentSystemAPI",
    "Audience": "PaymentSystemClients",
    "ExpiryInMinutes": 60
  },
  "AllowedHosts": "*"
}
```

### Key Configuration Options

| Setting | Purpose |
|---------|---------|
| `ConnectionStrings:DefaultConnection` | Database connection string (SQLite by default) |
| `JwtSettings:Secret` | Secret key for signing JWT tokens (must be ≥ 32 characters) |
| `JwtSettings:Issuer` | Token issuer identifier |
| `JwtSettings:Audience` | Intended token audience |
| `JwtSettings:ExpiryInMinutes` | Token validity duration in minutes |

### Environment-Specific Settings

- **Development** (`appsettings.Development.json`) - Used during development; overrides main settings
- **Production** - Update connection strings and JWT secret before deployment

---

## 📡 API Endpoints

### Authentication
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/auth/login` | User login (returns JWT token) |
| POST | `/api/auth/register` | Register new user account |

### Customers
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/customers` | Create new customer |
| GET | `/api/customers` | Get all customers |
| GET | `/api/customers/{id}` | Get customer by ID |
| DELETE | `/api/customers/{id}` | Delete customer |

### Accounts
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/accounts` | Create new account |
| GET | `/api/accounts` | Get all accounts |
| GET | `/api/accounts/{id}` | Get account details |
| GET | `/api/accounts/{id}/balance` | Get account balance |
| GET | `/api/accounts/customer/{customerId}` | Get account by customer |
| GET | `/api/accounts/balance-range` | Get accounts by balance range |
| PUT | `/api/accounts/{id}` | Update account |
| DELETE | `/api/accounts/{id}` | Delete account |

### Cards
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/cards` | Create new card |
| GET | `/api/cards` | Get all cards |
| GET | `/api/cards/{id}` | Get card by ID |
| GET | `/api/cards/account/{accountId}` | Get cards by account |
| PUT | `/api/cards/{id}` | Update card |
| DELETE | `/api/cards/{id}` | Delete card |

### Transactions
| Method | Endpoint | Description |
|--------|----------|-------------|
| POST | `/api/transactions` | Create new transaction |
| GET | `/api/transactions` | Get all transactions |
| GET | `/api/transactions/account/{accountId}` | Get transactions by account |
| GET | `/api/transactions/date-range` | Get transactions by date range |
| GET | `/api/transactions/type/{type}` | Get transactions by type |

### Admin
| Method | Endpoint | Description |
|--------|----------|-------------|
| GET | `/api/admin/dashboard` | Get admin dashboard data |
| GET | `/api/admin/admins` | Get all admin users |
| POST | `/api/admin/create` | Create new admin user |

### Swagger/OpenAPI
Access interactive API documentation at:
- `http://localhost:5000/swagger/v1/swagger.json` - OpenAPI specification
- `http://localhost:5000/` - Swagger UI (development only)

---

## 🔐 Authentication & Authorization

### JWT Token-Based Security

The API uses **JWT (JSON Web Tokens)** for authentication:

1. **Login** - User provides credentials to `/api/auth/login`
2. **Token Issued** - Server returns JWT token valid for 60 minutes
3. **Include Token** - Client includes token in `Authorization: Bearer {token}` header
4. **Validation** - Server validates token signature, issuer, audience, and expiration

### Role-Based Access Control (RBAC)

Users have roles defined in `UserRole` enum:
- **User** - Standard user with customer and account access
- **Admin** - Administrative access to dashboard and user management

### Security Headers

- `Authorization: Bearer {jwt-token}` - Required for protected endpoints
- Passwords are hashed using BCrypt with salt rounds
- Tokens require HTTPS in production (`RequireHttpsMetadata = false` for development)

---

## 🧪 Testing

### Running Tests

```bash
# Run all tests
dotnet test

# Run tests with coverage
dotnet test /p:CollectCoverage=true

# Run specific test class
dotnet test --filter "TestClassName=YourTestClass"

# Run tests in verbose mode
dotnet test --verbosity detailed
```

### Test Framework & Tools

- **xUnit** - Test framework for unit tests
- **Moq** - Mocking framework for creating mock objects
- **Coverlet** - Code coverage tool

### Test Organization

Tests are located in `payment-system.Tests/Services/` and follow the **Arrange-Act-Assert** pattern:

```csharp
[Fact]
public async Task CreateAccount_WithValidData_ReturnsSuccess()
{
    // Arrange
    var request = new CreateAccountRequest { /* ... */ };
    
    // Act
    var result = await _service.CreateAccount(request);
    
    // Assert
    Assert.NotNull(result);
}
```

---

## 📦 Deployment

### Building for Production

```bash
# Publish as self-contained deployment
dotnet publish -c Release -o ./publish

# Publish as framework-dependent deployment
dotnet publish -c Release -r win-x64 -o ./publish
```

### Database Considerations

#### SQLite (Development)
- Automatically created from migrations
- Located at `PaymentSystem.db`
- Good for development and small deployments

#### SQL Server (Production)
- Update connection string in `appsettings.json`
- Install SQL Server or use Azure SQL Database
- Run migrations on target database

### Environment Configuration

Set `ASPNETCORE_ENVIRONMENT` before running:
```bash
export ASPNETCORE_ENVIRONMENT=Production
dotnet run --project payment-system.Api
```

### Health Checks & Monitoring

- Monitor logs in `Logging.LogLevel`
- JWT token expiration should be monitored
- Database connection health should be verified

---

## 🤝 Contributing

### Contribution Guidelines

1. **Fork the repository** - Create your own copy
2. **Create a feature branch** - `git checkout -b feature/your-feature`
3. **Follow code standards**
   - Use PascalCase for classes and public methods
   - Use camelCase for local variables
   - Add XML documentation for public APIs
   - Keep methods focused and single-responsibility
4. **Add/Update Tests** - Ensure new features have test coverage
5. **Commit with clear messages** - `git commit -m "Add: description of changes"`
6. **Push and create a Pull Request** - Include description and related issues

### Code Style

- Use nullable reference types (enabled by default)
- Use async/await for I/O operations
- Prefer LINQ over loops
- Add meaningful comments for complex logic

---

## 📄 License

This project is licensed under the **MIT License**. See the LICENSE file for details.

---

## 📧 Support & Issues

- **Report Issues** - Use GitHub Issues for bug reports
- **Feature Requests** - Open a GitHub Discussion
- **Security Vulnerabilities** - Email privately (do not create public issues)

---

**Built with ❤️ using ASP.NET Core**
