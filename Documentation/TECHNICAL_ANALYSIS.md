  # Technical Analysis: PartStockManager

This document details the technological choices, software architecture, and technical initialization procedures for the **PartStockManager** application.

---

## 1. Technical Stack

* **Framework**: .NET 10.0 (C#)
* **Architecture**: N-Tier Architecture (API, CoreLogic, Adapter)
* **Data Access**: Entity Framework Core (Code-First Approach)
* **Database (Development & Testing)**: SQLite In-Memory
    * *Justification*: Using In-Memory mode ensures total test isolation and ultra-fast execution. The database is created and destroyed for each session, guaranteeing a consistently "clean" testing environment.
* **Logging**: Serilog
* **Testing**: xUnit, FluentAssertions, Microsoft.AspNetCore.Mvc.Testing

---

## 2. Prerequisites and Tooling

To manage the database and migrations, specific tools must be installed on the development machine.

### 2.1. EF Core Global Tool (CLI)
Before running migration commands, you must install the Entity Framework global tool. This command should be executed in a PowerShell terminal or a standard Command Prompt (CMD):

```powershell
dotnet tool install --global dotnet-ef
```

### 2.2. NuGet Packages
The following packages support persistence and logging. These can be installed via the NuGet Package Manager Console (Visual Studio) or the dotnet CLI.

Persistence (Adapter Project):

```powershell
dotnet add package Microsoft.EntityFrameworkCore.Sqlite
dotnet add package Microsoft.EntityFrameworkCore.Design
dotnet add package Microsoft.EntityFrameworkCore.Tools
```

Logging (API Project):

```powershell
dotnet add package Serilog.AspNetCore
dotnet add package Serilog.Sinks.File
dotnet add package Serilog.Sinks.Console
```

## 3. Entity Framework Core: Code-First Approach
The application utilizes the Code-First approach, where C# classes define the database schema.

Why this choice? It allows for total control over the business model within the code, letting EF Core generate the necessary SQL. This also simplifies schema change tracking through version control (Git).

Mapping: Database entities (PartEntity) are distinct from business models (Part) to adhere to the separation of concerns principle.

## 4. Migration Management
The database lifecycle is managed using the following commands in the Package Manager Console:

Generate a migration (Creates script files based on C# classes):
```
Add-Migration InitialCreation -Project PartStockManager.Adapter -StartupProject PartStockManager.API
```

Apply the migration (Creates the database and tables):
```
Update-Database -Project PartStockManager.Adapter -StartupProject PartStockManager.API
```

## 5. Software Architecture
### 5.1. API Layer (Presentation)
Entry Point: Uses public partial class Program to allow access from the integration testing project.

Middleware: Serilog is configured with Log.CloseAndFlush() within a finally block in Program.cs to ensure no logs are lost during application shutdown.

### 5.2. Adapter Layer (Infrastructure)
Repositories: Encapsulate LINQ queries. Searches are optimized for native SQL translation by EF Core.

## 6. Quality Strategy (Testing)
The project uses WebApplicationFactory for integration testing.

The API is hosted in-memory.

A dedicated SQLite In-Memory database is dynamically injected.

Application behavior is validated "end-to-end" (from the Controller down to the Persistence layer).
