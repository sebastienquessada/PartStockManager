# Changelog: PartStockManager

All notable changes to this project will be documented in this file using the version format: `[Major].[Year].[Month].[Day]`.

---

## [1.2026.4.12] - 2026-04-12
### Added
- **Base Architecture**: Implementation of the N-Tier structure using **.NET 10**.
- **Persistence Layer**: Configured Entity Framework Core (EF Code First).
- **Part Management**: Created the Repository with optimized LINQ filtering (Case-Insensitive search).
- **Unit Testing**: Implemented XUnit tests using **manual Stubs** to verify interface implementations in isolation.
- **Integration Testing**: Validated end-to-end flows (API + Database) using `WebApplicationFactory` and SQLite In-Memory.
- **Observability**: Integrated **Serilog** for structured logging (Console, File, and Debug sinks).
- **Documentation**: Drafted Functional Analysis, Technical Analysis, and English README.

---

## [Planned Evolutions]
- [ ] **Security**: Implementation of an authentication system.
- [ ] **User Management**: Creation of User entities and Role-Based Access Control (RBAC).
- [ ] **Frontend**: Development of a dedicated lightweight frontend (e.g., Angular or React) to communicate with the API.
