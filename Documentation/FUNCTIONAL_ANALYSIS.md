# Functional Analysis: PartStockManager

This document details the functional specifications and business rules for the **PartStockManager** application.

---

## 1. Global Description
**PartStockManager** is a solution designed to manage spare parts inventory: creation / creation / deletion of parts, parts search, inventory, stock entry / exit.

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
::--- | :--- | :--- |
| **Reference** | `string` | Unique identifier (e.g.: "PART-123") |
| **Name** | `string` | Part Name |
| **StockQuantity** | `int` | Quantity in stock |
| **LowStockThreshold** | `int` | Critical replenishment threshold |

---

## 4. Business Rules

1. **Rule No. 1 (Uniqueness)** : The part reference must be unique throughout the system.
2. **Rule No. 2 (Validity)** : A quantity in stock can never be negative.
3.  **Rule no. 3 (Consultation)** : If no filter is applied during a search, the system returns the entire catalog.

---

## 5. Architecture & Quality

* **Design API** - RESTful architecture using ASP.NET Core.
* **Persistence** - Use Entity Framework Core to ensure data integrity.
* **Validation** : Coverage by integration tests simulating real-world scenarios.
* **Traceability** : Structured logging (Serilog) to track every critical stage of the data lifecycle.
