# PartStockManager 🛠️

**PartStockManager** is a REST API developed in C# to manage a parts inventory.

## 🚀 Quick Start

### Prerequisites
- .NET 10 SDK
- EF Core Global Tool: 
  `dotnet tool install --global dotnet-ef`

### Installation
1. Clone the repository.
2. Apply migrations to initialize the database:
   `dotnet ef database update --project PartStockManager.Adapter --startup-project PartStockManager.API`
3. Run the application:
   `dotnet run --project PartStockManager.API`

## 🏗️ Architecture
The project follows an **N-Tier architecture** for a clear separation of concerns:
- **CoreLogic**: Entities and core business logic.
- **Adapter**: Infrastructure and persistence (EF Core).
- **API**: REST endpoints, configuration, and middleware.

## 🧪 Testing Strategy
- **Unit Tests**: Using custom **Stubs** to verify interface implementations and business logic in isolation.
- **Integration Tests**: Using `WebApplicationFactory` to validate real-world API flows and database interactions.

## 📊 Documentation
- [Functional Analysis](./FUNCTIONAL_ANALYSIS.md)
- [Technical Analysis](./TECHNICAL_ANALYSIS.md)
- [Changelog](./CHANGELOG.md)

## ⚖️ License
Distributed under the MIT License. See `LICENSE` for more information.
