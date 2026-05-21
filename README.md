# Nexora

Nexora is an ASP.NET Core API project with Entity Framework Core and PostgreSQL.

## Requirements

- .NET SDK 10
- PostgreSQL

## Getting Started

Restore dependencies and build the project:

```bash
dotnet restore
dotnet build
```

Run the API locally:

```bash
dotnet run --project Nexora
```

The application reads its database connection from the `DefaultConnection` connection string.
