# M3.Net - Modular Monolith Event Management Platform

A modern, scalable .NET 8 application built using **Modular Monolith** architecture with **Clean Architecture** principles, **Domain-Driven Design**, and **Event-Driven Architecture**.

## Architecture Overview

M3.Net is designed as a **Modular Monolith** where each module operates as an independent, interchangeable component while being deployed as a single application. This approach provides the benefits of microservices (modularity, bounded contexts) while maintaining the simplicity of monolithic deployment.

### Core Architectural Principles

- **Logical Isolation**: Each module is completely isolated with its own domain, application, and infrastructure layers
- **Public API Communication**: Modules communicate exclusively through public APIs and distributed messaging
- **Database Isolation**: Each module has its own database schema ensuring bounded contexts
- **Clean Architecture**: All modules follow Clean Architecture with strict layer dependencies
- **Event-Driven**: Asynchronous communication via distributed message broker (RabbitMQ)

## Module Structure

Each module follows the **Clean Architecture** pattern with four distinct layers:

```
src/Modules/{ModuleName}/
├── M3.Net.Modules.{ModuleName}.Domain/          # Domain Layer
│   ├── Entities/                                # Domain entities
│   ├── Events/                                  # Domain events
│   ├── Interfaces/                              # Repository contracts
│   └── Errors/                                  # Domain-specific errors
├── M3.Net.Modules.{ModuleName}.Application/     # Application Layer
│   ├── Commands/                                # CQRS Commands
│   ├── Queries/                                 # CQRS Queries
│   ├── Handlers/                                # Command/Query handlers
│   ├── Validators/                              # FluentValidation rules
│   └── Abstractions/                            # Application interfaces
├── M3.Net.Modules.{ModuleName}.Infrastructure/  # Infrastructure Layer
│   ├── Database/                                # EF Core DbContext & migrations
│   ├── Repositories/                            # Repository implementations
│   ├── Services/                                # External service integrations
│   ├── Outbox/                                  # Outbox pattern implementation
│   └── Inbox/                                   # Inbox pattern implementation
├── M3.Net.Modules.{ModuleName}.Presentation/    # Presentation Layer
│   ├── Endpoints/                               # Minimal API endpoints
│   └── Models/                                  # Request/Response DTOs
├── M3.Net.Modules.{ModuleName}.IntegrationEvents/ # Integration Events
└── Tests/                                       # Test projects
    ├── UnitTests/
    ├── IntegrationTests/
    └── ArchitectureTests/
```

## Current Modules

### Users Module
**Bounded Context**: User Management & Authentication
- User registration and profile management
- Role-based access control (RBAC)
- Permission management
- Keycloak integration for identity provider
- **Database Schema**: `users`

### Transcripts Module *(In Development)*
**Bounded Context**: Document/Transcript Management
- *Requires event storming to define complete bounded context*
- **Database Schema**: `transcripts`

## Clean Architecture Implementation

### Dependency Flow
```
Presentation → Application → Domain
Infrastructure → Application → Domain
```

### Layer Responsibilities

**Domain Layer**
- Business entities and value objects
- Domain events and business rules
- Repository interfaces
- Domain services and specifications

**Application Layer**
- Use cases (Commands/Queries)
- Application services
- Integration event handlers
- Cross-cutting concerns (validation, caching)

**Infrastructure Layer**
- Data access implementation
- External service integrations
- Message broker configuration
- Authentication and authorization

**Presentation Layer**
- API endpoints and controllers
- Request/response models
- API documentation

## Inter-Module Communication

### Synchronous Communication
- **Public APIs**: Modules expose REST endpoints for external communication
- **No Direct Database Access**: Modules cannot access other modules' databases

### Asynchronous Communication
- **Integration Events**: Published via RabbitMQ using MassTransit
- **Outbox Pattern**: Ensures reliable event publishing
- **Inbox Pattern**: Ensures reliable event processing
- **Message Deduplication**: Idempotent event handlers prevent duplicate processing

## Data Architecture

### Database Isolation Strategy
- **Schema-Per-Module**: Each module has its own PostgreSQL schema
- **Bounded Contexts**: Data models are isolated within module boundaries
- **No Cross-Schema Queries**: Modules cannot query other modules' data directly
- **Event Sourcing Ready**: Architecture supports future event sourcing implementation

### Current Schemas
- `users` - User management and authentication
- `transcripts` - Document and transcript management *(planned)*

## Technology Stack

### Core Framework
- **.NET 8** - Latest LTS framework
- **ASP.NET Core** - Web API framework
- **Entity Framework Core** - ORM with PostgreSQL provider

### Architecture & Patterns
- **MediatR** - CQRS implementation
- **FluentValidation** - Input validation
- **MassTransit** - Message broker abstraction
- **Quartz.NET** - Background job scheduling

### Infrastructure
- **PostgreSQL** - Primary database
- **RabbitMQ** - Message broker
- **Redis** - Caching layer
- **Keycloak** - Identity and access management
- **Docker** - Containerization

### Quality & Monitoring
- **OpenTelemetry** - Observability and tracing
- **Serilog** - Structured logging
- **HealthChecks** - Application health monitoring
- **NetArchTest** - Architecture rule enforcement

## Testing Strategy

### Test Types
- **Unit Tests** - Domain logic and business rules
- **Integration Tests** - API endpoints and database operations
- **Architecture Tests** - Enforce architectural constraints and naming conventions

### Architecture Rules Enforced
- Layer dependency validation
- Naming convention compliance
- Public/private access modifiers
- Interface implementations

## Getting Started

### Prerequisites
- .NET 8 SDK
- Docker and Docker Compose
- PostgreSQL (or use Docker)
- RabbitMQ (or use Docker)
- Redis (or use Docker)

### Quick Start
```bash
# Clone the repository
git clone <repository-url>
cd m3.net

# Start infrastructure services
docker-compose up -d

# Run the application
dotnet run --project src/API/M3.Net.Api

# Run tests
dotnet test
```

### Configuration
Configure connection strings in `appsettings.json`:
```json
{
  "ConnectionStrings": {
    "Database": "Host=localhost;Database=m3net;Username=postgres;Password=postgres",
    "Cache": "localhost:6379",
    "Queue": "amqp://guest:guest@localhost:5672/"
  }
}
```

## Module Development Guidelines

### Adding a New Module

1. **Event Storming Session**
   - Define bounded context boundaries
   - Identify domain events and aggregates
   - Map business processes and workflows

2. **Create Module Structure**
   - Follow the established folder structure
   - Implement Clean Architecture layers
   - Define database schema

3. **Integration Points**
   - Create integration events for cross-module communication
   - Implement public API endpoints
   - Configure message handlers

4. **Testing**
   - Write comprehensive unit tests
   - Create integration tests for APIs
   - Add architecture tests for rule enforcement

### Architectural Constraints

**DO**
- Keep modules logically isolated
- Use integration events for cross-module communication
- Follow Clean Architecture dependency rules
- Implement comprehensive testing

**DON'T**
- Reference other modules directly
- Share database schemas between modules
- Bypass the public API for module communication
- Violate layer dependency rules

## Future Roadmap

- **Event Sourcing**: Implement event sourcing for audit and replay capabilities
- **CQRS Read Models**: Separate read models for optimized queries
- **Module Deployment**: Extract modules into separate deployable units
- **Advanced Monitoring**: Enhanced observability and metrics
- **API Versioning**: Support for API evolution and backward compatibility

## Contributing

1. Follow the established architectural patterns
2. Ensure all tests pass
3. Maintain Clean Architecture principles
4. Document new modules and features
5. Conduct event storming for new bounded contexts

## License

[Add your license information here]

---

**M3.Net** - Building scalable, maintainable software through modular architecture and clean design principles.
