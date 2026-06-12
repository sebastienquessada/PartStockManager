# Functional Analysis: PartStockManager

This document details the functional specifications and business rules for the **PartStockManager** application.

---

## 1. Global Description
**PartStockManager** is a solution designed to manage spare parts inventory: creation / modification / deletion of parts, parts search, inventory, stock entry / exit.

---

## 2. Functional Specifications

### 2.1. Managing the Parts Catalog
The system must be able to store and view essential information for each part:
* **Unique identification** : Each part is identified by a strict alphanumeric reference.
* **Label** : A descriptive name to facilitate human recognition.
* **Stock status** : The physical quantity currently available in store.
* **Alert threshold** : A customizable limit value for each reference.

### 2.2. Advanced search engine
The API exposes filtering capabilities for navigating large catalogs:
* **Search by name** : Partial search (e.g. "bolt" will find "steel bolt" and "zinc bolt").
* **Search by reference** : Targeted search on the manufacturer code.
* **Normalization** : Search is case insensitive to avoid user input errors.

### 2.3. Threshold (Low Stock) Analysis
The system must be able to dynamically calculate the list of parts requiring immediate attention. 
A part is considered to be in "Low Stock" if it meets the following logical condition:
$$StockQuantity \le LowStockThreshold$$

---

## 3. Data Model

| Attribute | Type | Description |
| :--- | :--- | :--- |
| **Reference** | `string` | Unique identifier (e.g.: "PART-123") |
| **Name** | `string` | Part Name |
| **StockQuantity** | `int` | Quantity in stock |
| **LowStockThreshold** | `int` | Critical replenishment threshold |

---

## 4. Business Rules

1. **Rule No. 1 (Uniqueness)** : The part reference must be unique throughout the system.
2. **Rule No. 2 (Validity)** : A quantity in stock can never be negative.
3. **Rule No. 3 (Consultation)** : All parts in the catalog are accessible by default. Filters are optional and narrow down the results.

---

## 5. Architecture & Quality

* **Design API** - RESTful architecture using ASP.NET Core.
* **Persistence** - Use Entity Framework Core to ensure data integrity.
* **Validation** : Coverage by integration tests simulating real-world scenarios.
* **Traceability** : Structured logging (Serilog) to track every critical stage of the data lifecycle.

---

## 6. User Management & Access Rights

### 6.1. User Profiles
Each user account is assigned exactly one profile, which determines the set of actions they are authorized to perform:

| Profile | Description |
| :--- | :--- |
| **Administrator** | Full access to the system, including user management. Cannot modify their own rights. |
| **Manager** | Can create, modify, and delete parts, and perform stock entries, exits, and inventories. |
| **Stocktaker** | Can only perform inventories. |

### 6.2. Permissions Matrix

| Right | Administrator | Manager | Stocktaker |
| :--- | :---: | :---: | :---: |
| Create/modify/delete users | ✅ | ❌ | ❌ |
| Consult users | ✅ | ❌ | ❌ |
| Create/modify/delete parts | ✅ | ✅ | ❌ |
| Consult parts | ✅ | ✅ | ✅ |
| Stock entries/exits | ✅ | ✅ | ❌ |
| Inventory | ✅ | ✅ | ✅ |

### 6.3. Account Rules

1. **Rule No. 4 (Self-Rights Protection)** : An Administrator cannot modify their own profile, to prevent from accidental privilege loss.
2. **Rule No. 5 (Default Administrator)** : A default administrator account is created on first startup, ensuring the system is always accessible. This account cannot be modified or deleted, only its password can be changed.
3. **Rule No. 6 (Self-Deletion Protection)** : An Administrator cannot delete his/her own account.
4. **Rule No. 7 (Password Management)** : Any authenticated user can change their own password by providing their current password. An Administrator can additionally change any other user's password without knowing the current one.
