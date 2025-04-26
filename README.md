# 📚 Project Structure - Mini Banking System

This document describes the folder and file structure of the `sovannarapov-mini-banking-system` project, organized following **Clean Architecture** principles.

---

## 🗂️ Directory Structure

### `src/`
Contains all production code, divided by logical layers.

---

### 🔹 `Application/`

- **Abstractions/**: Pipeline behaviors, interfaces for data access and messaging
- **Common/**: Core service interfaces and mappings
- **Dtos/**: Request/response models
- **Features/**: CQRS Commands, Queries, Handlers for Accounts and Transactions
- **Interfaces/**: Utility interfaces (e.g., `IGuidGenerator`)
- **Services/**: Implementations of utility services
- **DependencyInjection.cs**: Registers Application services for DI

---

### 🔹 `Domain/`

- **Accounts/**: Account entity and related enums
- **Transactions/**: Transaction entity and related enums
- **Constants/**: Centralized constant values
- **Extensions/**: Helper methods (e.g., Enum extensions)

---

### 🔹 `Infrastructure/`

- **Database/**: EF Core DbContext and Migrations
- **Services/**: Concrete service implementations (e.g., `AccountService`, `TransactionService`)
- **Configurations/**: EF Core entity configurations
- **DependencyInjection.cs**: Registers Infrastructure services for DI

---

### 🔹 `Shared/`

- **Shared result types and error handling:**
- Result.cs, Error.cs, ErrorType.cs, ValidatorError.cs

---

### 🔹 `Web.Api/`

- **Endpoints/**: Minimal API endpoints (Accounts, Transactions)
- **Extensions/**: ApplicationBuilder and ServiceCollection extensions
- **Mappings/**: API request DTO mappings
- **Middleware/**: Custom middleware (e.g., Logging)
- **Infrastructure/**: Custom API Results
- **Constants/**: Swagger related constants
- **Program.cs**, **DependencyInjection.cs**: API initialization and service setup
- **Dockerfile**: Containerization support
- **HTTP Request Files**: Example requests (`requests.http`)

---

## 🧪 Tests

### `tests/`

- **Unit/**: Unit tests for each layer and feature
    - **Common/**: Shared base test classes (`BaseTest.cs`, `GlobalUsings.cs`)
    - **Features/**
        - **Accounts/**
            - `CreateAccountCommandHandlerTests.cs`
            - `GetAccountByIdQueryHandlerTests.cs`
            - `GetAccountQueryHandlerTests.cs`
        - **Transactions/**
            - `DepositTransactionCommandHandlerTests.cs`
            - `WithdrawTransactionCommandHandlerTests.cs`
    - **Infrastructure/**:
        - `LayerTest.cs`: Infrastructure Layer tests

---

## 🔄 CI/CD - `.github/`

GitHub Actions workflows for automating:

- `ci.yml`: Continuous Integration
- `cd.yml`: Continuous Deployment

---

# ✅ Summary

This project follows:

- Clean Architecture 🧹
- Separation of Concerns 📚
- Test-Driven Development (TDD) 🧪
- SOLID Principles 🛠️
