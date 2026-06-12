# Changelog: PartStockManager

All notable changes to this project will be documented in this file using the version format: `[Major].[Year].[Month].[Day]`.

---

## [1.2026.6.12] - 2026-06-12
### Added
- **Authentication**: Implemented JWT (JSON Web Token) Bearer authentication via `POST /api/Auth/login`.
- **Password Security**: Integrated BCrypt for password hashing and verification.
- **User Management**: Created `User`/`UserProfile` models, `IUserRepository`/`UserRepository`, and `UserController` with full CRUD operations (create, modify, delete, list).
- **Role-Based Access Control (RBAC)**: Defined three profiles (Administrator, Manager, Stocktaker) with a permissions matrix enforced via `[Authorize(Roles = "...")]` on all controller actions.
- **Account Protections**:
  - Administrators cannot modify their own profile/rights.
  - Administrators cannot delete their own account.
  - The default administrator account (created on startup) cannot be modified or deleted.
- **Password Self-Service**: Added `PUT /api/User/change-password`, allowing any authenticated user to change their own password, and Administrators to change any user's password.
- **Default Administrator**: Automatic creation of a default administrator account on first startup (configurable via `Seed:AdminUsername`/`Seed:AdminPassword`), skipped in the `Testing` environment.
- **Integration Testing**: Added `GenerateTestToken` helper in `CustomWebApplicationFactory` to generate role-specific JWTs for testing authorization rules across `PartController`, `StockController`, and `UserController`.

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
- [ ] **Frontend**: Development of a dedicated lightweight frontend (e.g., Angular or React) to communicate with the API.
