# Nexora

Nexora is a modern microservices-based financial API built with **ASP.NET Core 10**, **Entity Framework Core**, **PostgreSQL**, and **RabbitMQ**. It is designed using **Clean Architecture** principles and includes a distributed messaging system for processing financial events asynchronously.

## System Architecture

The project consists of two main microservices that communicate via RabbitMQ:

1. **Nexora.API**: The main REST API handling user registration, authentication, deposits, transfers, and transaction history.
2. **Nexora.TaxInspection**: A background worker service that listens to transaction events and calculates hypothetical taxes on transfers.

```mermaid
flowchart TD
    User([User / Postman]) --> API[Nexora.API (REST)]
    API <--> DB[(PostgreSQL)]
    
    API -- "Publishes TransactionCreatedEvent" --> RMQ{RabbitMQ}
    RMQ -- "Consumes TransactionCreatedEvent" --> TAX[Nexora.TaxInspection]
    
    TAX --> Log[Console / Logger]
```

### Clean Architecture Layers

The solution is divided into the following layers to ensure separation of concerns:

- **Nexora.Domain**: Enterprise entities (`User`, `Account`, `Transaction`, `Session`) and Domain Events.
- **Nexora.Application**: Business logic, CQRS Handlers (MediatR), Validators (FluentValidation), and Interfaces.
- **Nexora.Infrastructure**: Data access (EF Core Repositories), Messaging implementations (RabbitMQ Producer), and Database Queries.
- **Nexora.API**: Controllers, Middlewares, and HTTP pipeline.

## Technologies Used

- **.NET 10** (C#)
- **ASP.NET Core Web API**
- **Entity Framework Core 10** (Npgsql)
- **PostgreSQL 16**
- **RabbitMQ 3**
- **MediatR** (CQRS pattern)
- **FluentValidation**
- **AutoMapper**
- **Docker & Docker Compose**

---

## Quick Start (Docker)

The easiest way to run the entire system (API, PostgreSQL, RabbitMQ, and the Tax Inspection worker) is using Docker Compose.

1. Clone the repository.
2. Ensure Docker and Docker Compose are installed and running.
3. Run the following command in the root directory:

```powershell
docker compose up -d --build
```

The system will start 4 containers:
- `nexora-postgres-1` (Port: 5434)
- `nexora-rabbitmq-1` (Ports: 5672, 15672)
- `nexora-app-1` (API - Port: 5196)
- `nexora-tax-inspection-1` (Background Consumer)

> **Note:** The API and Tax Inspection services are configured to wait for PostgreSQL and RabbitMQ to be `healthy` before starting. 

### Endpoints and Access

- **Swagger UI**: [http://localhost:5196/swagger](http://localhost:5196/swagger)
- **RabbitMQ Management UI**: [http://localhost:15672](http://localhost:15672) *(guest / guest)*

---

## API Endpoints

### User Management
| Method | Endpoint | Auth | Body | Description |
|---|---|---|---|---|
| `POST` | `/api/user/register` | No | `{ "login", "name", "password" }` | Register a new user |
| `POST` | `/api/user/login` | No | `{ "login", "password" }` | Login and get JWT token |

### Finance Operations
| Method | Endpoint | Auth | Body / Query | Description |
|---|---|---|---|---|
| `GET` | `/api/finance/balance` | Yes | - | Get current balance |
| `POST` | `/api/finance/deposit` | Yes | `{ "amount", "currency" }` | Deposit funds |
| `POST` | `/api/finance/transfer` | Yes | `{ "receiverLogin", "amount", "currency" }` | Transfer funds to another user |
| `GET` | `/api/finance/history` | Yes | `?offset=0&limit=20` | Get transaction history |

*(Authentication is done via `Authorization: Bearer <token>` header)*

---

## Example Usage Workflow

1. **Register two users:**
```bash
curl -X POST "http://localhost:5196/api/User/register" -H "Content-Type: application/json" -d "{\"login\":\"user1\", \"name\":\"User One\", \"password\":\"pass123\"}"
curl -X POST "http://localhost:5196/api/User/register" -H "Content-Type: application/json" -d "{\"login\":\"user2\", \"name\":\"User Two\", \"password\":\"pass123\"}"
```

2. **Login as user1:**
```bash
curl -X POST "http://localhost:5196/api/User/login" -H "Content-Type: application/json" -d "{\"login\":\"user1\", \"password\":\"pass123\"}"
# Copy the returned token
```

3. **Deposit money to user1:**
```bash
curl -X POST "http://localhost:5196/api/Finance/deposit" -H "Authorization: Bearer YOUR_TOKEN" -H "Content-Type: application/json" -d "{\"amount\":1000, \"currency\":\"RUB\"}"
```

4. **Transfer to user2:**
```bash
curl -X POST "http://localhost:5196/api/Finance/transfer" -H "Authorization: Bearer YOUR_TOKEN" -H "Content-Type: application/json" -d "{\"receiverLogin\":\"user2\", \"amount\":150, \"currency\":\"RUB\"}"
```

5. **Check Tax Inspection Logs:**
```bash
docker compose logs tax-inspection
```
*You will see that the microservice intercepted the transfer event via RabbitMQ and processed the hypothetical tax!*

---

## Development & Security Features Included

- **Password Hashing:** PBKDF2 with random salts and 100,000 iterations.
- **Timing Attack Prevention:** Uses `CryptographicOperations.FixedTimeEquals` for hash comparison.
- **Concurrency Protection:** Uses EF Core's `ExecuteUpdateAsync` for atomic balance modifications.
- **Connection Pooling:** Singleton `IConnection` for RabbitMQ to prevent port exhaustion.
- **Fault Tolerance:** 2-second reconnect retry policy on RabbitMQ consumers.
- **N+1 Query Prevention:** Transaction history uses proper EF Core navigation property joins instead of correlated subqueries.
