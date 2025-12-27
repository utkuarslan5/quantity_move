# Quantity Move API - Complete Documentation

## Table of Contents
1. [Architecture Overview](#architecture-overview)
2. [API Methods List](#api-methods-list)
3. [Service Architecture](#service-architecture)
4. [Workflows](#workflows)
5. [Design Principles](#design-principles)

---

## Architecture Overview

The Quantity Move API follows a **modular architecture** with clear separation of concerns. Each service has a single responsibility, and methods are focused on specific tasks rather than being overloaded with multiple responsibilities.

### High-Level Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                      API Controllers                         │
│  ┌──────────┐  ┌──────────┐  ┌──────────┐  ┌──────────┐   │
│  │  Stock   │  │ Quantity │  │   FIFO   │  │ Barcode  │   │
│  │Controller│  │Controller│  │Controller│  │Controller│   │
│  └────┬─────┘  └────┬─────┘  └────┬─────┘  └────┬─────┘   │
└───────┼─────────────┼─────────────┼─────────────┼─────────┘
        │             │             │             │
        ▼             ▼             ▼             ▼
┌─────────────────────────────────────────────────────────────┐
│                    Service Layer                            │
│  ┌──────────────┐  ┌──────────────┐  ┌──────────────┐    │
│  │ Stock        │  │ Quantity     │  │ FIFO         │    │
│  │ Services     │  │ Services     │  │ Service      │    │
│  │              │  │              │  │              │    │
│  │ - Query      │  │ - Move       │  │ - Validation │    │
│  │ - Validation │  │ - Validation │  │ - Summary    │    │
│  │ - Location   │  │              │  │              │    │
│  └──────────────┘  └──────────────┘  └──────────────┘    │
│                                                             │
│  ┌──────────────┐  ┌──────────────────────────────────┐  │
│  │ Barcode      │  │ Composition Service               │  │
│  │ Service      │  │ (Combines multiple services)    │  │
│  └──────────────┘  └──────────────────────────────────┘  │
└─────────────────────────────────────────────────────────────┘
        │
        ▼
┌─────────────────────────────────────────────────────────────┐
│                  Database Service Layer                     │
│  ┌────────────────────────────────────────────────────┐   │
│  │         DatabaseService (Dapper)                   │   │
│  │  - Stored Procedure Execution                      │   │
│  │  - Query Execution                                 │   │
│  └────────────────────────────────────────────────────┘   │
└─────────────────────────────────────────────────────────────┘
        │
        ▼
┌─────────────────────────────────────────────────────────────┐
│                      SQL Server Database                    │
│  - lot_loc_mst (Lot Location Inventory)                    │
│  - item_mst (Item Master)                                   │
│  - location_mst (Location Master)                          │
│  - lot_mst (Lot Master)                                     │
│  - fifo_summary (FIFO Summary)                             │
└─────────────────────────────────────────────────────────────┘
```

---

## API Methods List

### Stock Controller (`/api/stock`)

#### Query Endpoints

| Method | Endpoint | Description | Parameters |
|--------|----------|-------------|------------|
| `GET` | `/api/stock/quantity` | Get quantity at specific location | `itemCode`, `lotNumber`, `locationCode`, `warehouseCode` |
| `GET` | `/api/stock/locations` | Get all locations for item/lot | `itemCode`, `lotNumber`, `warehouseCode`, `includeZeroQuantity` |
| `GET` | `/api/stock/locations/with-stock` | Get locations with stock | `itemCode`, `lotNumber`, `warehouseCode` |
| `GET` | `/api/stock/current-location` | Get current location(s) for item/lot | `itemCode`, `lotNumber`, `warehouseCode` |
| `GET` | `/api/stock/summary` | Get stock summary for item | `itemCode`, `warehouseCode` |

#### Validation Endpoints

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `POST` | `/api/stock/validate/item` | Validate item exists and is lot-tracked | `ValidateItemRequest` |
| `POST` | `/api/stock/validate/lot` | Validate lot exists | `ValidateLotRequest` |
| `POST` | `/api/stock/validate/location` | Validate location exists | `ValidateLocationRequest` |
| `POST` | `/api/stock/validate/availability` | Validate stock availability | `ValidateStockRequest` |
| `POST` | `/api/stock/validate/move` | Combined validation for move operation | `StockMoveValidationRequest` |

### Quantity Controller (`/api/quantity`)

#### Validation Endpoints (Pre-Move)

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `POST` | `/api/quantity/validate/source` | Validate source location has sufficient stock | `ValidateSourceRequest` |
| `POST` | `/api/quantity/validate/target` | Validate target location is valid | `ValidateTargetRequest` |
| `POST` | `/api/quantity/validate/move` | Validate move is allowed | `MoveValidationRequest` |

#### Move Endpoints

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `POST` | `/api/quantity/move` | Simple move operation | `MoveQuantityRequest` |
| `POST` | `/api/quantity/move/with-validation` | Move with validation | `MoveQuantityRequest` |
| `POST` | `/api/quantity/move/with-fifo` | Move with FIFO check | `MoveQuantityRequest` |

### FIFO Controller (`/api/fifo`)

| Method | Endpoint | Description | Parameters/Body |
|--------|----------|-------------|-----------------|
| `GET` | `/api/fifo/oldest-lot` | Get oldest lot for item | `itemCode`, `warehouseCode`, `siteReference` |
| `POST` | `/api/fifo/validate` | Check FIFO compliance | `FifoValidationRequest` |
| `GET` | `/api/fifo/summary` | Get FIFO summary for item | `itemCode`, `warehouseCode`, `siteReference` |

### Barcode Controller (`/api/barcode`)

| Method | Endpoint | Description | Request Body |
|--------|----------|-------------|--------------|
| `POST` | `/api/barcode/parse` | Parse barcode string (item%lot%quantity) | `string` |
| `POST` | `/api/barcode/lookup` | Lookup item/lot from barcode | `string` |

---

## Service Architecture

### Service Layer Breakdown

```
IStockService (Core Stock Operations)
├── IStockQueryService (Read Operations)
│   ├── GetQuantityAtLocationAsync
│   ├── GetLocationsForItemLotAsync
│   ├── GetStockSummaryAsync
│   └── GetCurrentLocationAsync
│
├── IStockValidationService (Validation Operations)
│   ├── ValidateItemAsync
│   ├── ValidateLotAsync
│   ├── ValidateLocationAsync
│   ├── ValidateStockAvailabilityAsync
│   └── ValidateStockForMoveAsync (Composition)
│
└── IStockLocationService (Location-Specific Operations)
    ├── GetLocationsWithStockAsync
    ├── GetLocationsWithoutStockAsync
    └── GetLocationDetailsAsync

IQuantityService (Quantity Movement)
├── IQuantityMoveService (Move Operations)
│   ├── MoveQuantityAsync
│   ├── MoveQuantityWithValidationAsync (Composition)
│   └── MoveQuantityWithFifoCheckAsync (Composition)
│
└── IQuantityValidationService (Pre-Move Validation)
    ├── ValidateSourceStockAsync
    ├── ValidateTargetLocationAsync
    └── ValidateMoveAsync (Composition)

IFifoService (FIFO Operations)
├── GetOldestLotAsync
├── ValidateFifoComplianceAsync
└── GetFifoSummaryAsync

IBarcodeService (Barcode Operations)
├── ParseBarcodeAsync
└── LookupBarcodeAsync

IStockOperationService (Composition Service)
└── MoveQuantityWithFullValidationAsync
    (Combines: Validation + FIFO + Move)
```

### Service Dependencies

```mermaid
graph TD
    StockController --> IStockQueryService
    StockController --> IStockValidationService
    StockController --> IStockLocationService
    
    QuantityController --> IQuantityMoveService
    QuantityController --> IQuantityValidationService
    
    FifoController --> IFifoService
    BarcodeController --> IBarcodeService
    
    IQuantityMoveService --> IQuantityValidationService
    IQuantityMoveService --> IFifoService
    IQuantityMoveService --> IDatabaseService
    
    IStockValidationService --> IDatabaseService
    IStockQueryService --> IDatabaseService
    IStockLocationService --> IDatabaseService
    IFifoService --> IDatabaseService
    IBarcodeService --> IDatabaseService
    
    IStockOperationService --> IStockValidationService
    IStockOperationService --> IQuantityMoveService
    IStockOperationService --> IFifoService
    
    IDatabaseService --> SQLServer[SQL Server Database]
```

---

## Workflows

### 1. Stock Validation Workflow

**Purpose**: Validate that stock is available and valid before performing operations.

```mermaid
sequenceDiagram
    participant Client
    participant StockController
    participant StockValidationService
    participant DatabaseService
    participant Database

    Client->>StockController: POST /api/stock/validate/move
    StockController->>StockValidationService: ValidateStockForMoveAsync
    
    StockValidationService->>StockValidationService: ValidateItemAsync
    StockValidationService->>DatabaseService: Query item_mst
    DatabaseService->>Database: SELECT item, lot_tracked
    Database-->>DatabaseService: Item data
    DatabaseService-->>StockValidationService: Item validation result
    
    StockValidationService->>StockValidationService: ValidateLotAsync
    StockValidationService->>DatabaseService: Query lot_mst
    DatabaseService->>Database: SELECT lot
    Database-->>DatabaseService: Lot data
    DatabaseService-->>StockValidationService: Lot validation result
    
    StockValidationService->>StockValidationService: ValidateLocationAsync (Source)
    StockValidationService->>DatabaseService: Query location_mst
    DatabaseService->>Database: SELECT loc, uf_yer_tipi
    Database-->>DatabaseService: Location data
    DatabaseService-->>StockValidationService: Source location validation
    
    StockValidationService->>StockValidationService: ValidateLocationAsync (Target)
    StockValidationService->>DatabaseService: Query location_mst
    DatabaseService->>Database: SELECT loc, uf_yer_tipi
    Database-->>DatabaseService: Location data
    DatabaseService-->>StockValidationService: Target location validation
    
    StockValidationService->>StockValidationService: ValidateStockAvailabilityAsync
    StockValidationService->>DatabaseService: Query lot_loc_mst
    DatabaseService->>Database: SELECT qty_on_hand
    Database-->>DatabaseService: Quantity data
    DatabaseService-->>StockValidationService: Availability result
    
    StockValidationService->>StockValidationService: Combine all validations
    StockValidationService-->>StockController: CombinedValidationResponse
    StockController-->>Client: API Response
```

**Steps**:
1. **Validate Item**: Check if item exists and is lot-tracked
2. **Validate Lot**: Verify lot number exists for the item
3. **Validate Source Location**: Ensure source location exists and is valid
4. **Validate Target Location**: Ensure target location exists and is valid
5. **Validate Stock Availability**: Check if sufficient quantity exists at source
6. **Combine Results**: Return comprehensive validation response

---

### 2. Quantity Move with Full Validation Workflow

**Purpose**: Move quantity from source to target location with complete validation.

```mermaid
sequenceDiagram
    participant Client
    participant QuantityController
    participant QuantityMoveService
    participant QuantityValidationService
    participant StockValidationService
    participant FifoService
    participant DatabaseService
    participant Database

    Client->>QuantityController: POST /api/quantity/move/with-fifo
    QuantityController->>QuantityMoveService: MoveQuantityWithFifoCheckAsync
    
    QuantityMoveService->>FifoService: ValidateFifoComplianceAsync
    FifoService->>DatabaseService: Query fifo_summary
    DatabaseService->>Database: SELECT oldest lot
    Database-->>DatabaseService: FIFO data
    DatabaseService-->>FifoService: FIFO validation result
    FifoService-->>QuantityMoveService: FIFO compliance status
    
    QuantityMoveService->>QuantityValidationService: ValidateMoveAsync
    QuantityValidationService->>QuantityValidationService: ValidateSourceStockAsync
    QuantityValidationService->>DatabaseService: Query lot_loc_mst
    DatabaseService->>Database: SELECT qty_on_hand
    Database-->>DatabaseService: Source quantity
    DatabaseService-->>QuantityValidationService: Source validation
    
    QuantityValidationService->>QuantityValidationService: ValidateTargetLocationAsync
    QuantityValidationService->>DatabaseService: Query location_mst
    DatabaseService->>Database: SELECT loc
    Database-->>DatabaseService: Location data
    DatabaseService-->>QuantityValidationService: Target validation
    
    QuantityValidationService-->>QuantityMoveService: Move validation result
    
    QuantityMoveService->>DatabaseService: Execute move_quantity SP
    DatabaseService->>Database: EXEC move_quantity
    Database-->>DatabaseService: Transaction ID, Return Code
    DatabaseService-->>QuantityMoveService: Move result
    
    QuantityMoveService-->>QuantityController: MoveQuantityResponse
    QuantityController-->>Client: API Response
```

**Steps**:
1. **FIFO Check**: Validate that oldest lot is being used (if enabled)
2. **Source Validation**: Verify source has sufficient stock
3. **Target Validation**: Verify target location is valid
4. **Execute Move**: Call stored procedure to perform the move
5. **Return Result**: Provide transaction ID and status

---

### 3. FIFO Compliance Check Workflow

**Purpose**: Ensure First-In-First-Out compliance for lot tracking.

```mermaid
flowchart TD
    Start([FIFO Validation Request]) --> GetOldestLot[Get Oldest Lot from fifo_summary]
    GetOldestLot --> CheckExists{Oldest Lot<br/>Exists?}
    
    CheckExists -->|No| Compliant[Return: Compliant<br/>No older lots]
    CheckExists -->|Yes| CompareLots{Current Lot ==<br/>Oldest Lot?}
    
    CompareLots -->|Yes| Compliant
    CompareLots -->|No| Warning[Return: Non-Compliant<br/>Warning Message]
    
    Compliant --> End([Return Validation Response])
    Warning --> End
    
    style Compliant fill:#90EE90
    style Warning fill:#FFB6C1
```

**Explanation**:
- Queries `fifo_summary` table to find oldest lot for the item
- Compares current lot with oldest lot
- Returns compliance status and warning if non-compliant
- Used before quantity moves to enforce FIFO policy

---

### 4. Barcode Parsing and Lookup Workflow

**Purpose**: Parse barcode format and lookup item information.

```mermaid
flowchart TD
    Start([Barcode String]) --> Parse[Split by '%' delimiter]
    Parse --> CheckFormat{Format Valid?<br/>item%lot%quantity}
    
    CheckFormat -->|No| Error1[Return: Invalid Format]
    CheckFormat -->|Yes| Extract[Extract: Item, Lot, Quantity]
    
    Extract --> Lookup[Lookup Item in item_mst]
    Lookup --> CheckFound{Item Found?}
    
    CheckFound -->|No| Error2[Return: Item Not Found]
    CheckFound -->|Yes| Success[Return: Item Info + Lot + Quantity]
    
    Error1 --> End([Response])
    Error2 --> End
    Success --> End
    
    style Success fill:#90EE90
    style Error1 fill:#FFB6C1
    style Error2 fill:#FFB6C1
```

**Barcode Format**: `ITEM_CODE%LOT_NUMBER%QUANTITY`
- Example: `ABC123%LOT001%100.50`
- Quantity is optional

---

## Design Principles

### 1. Single Responsibility Principle
Each service and method has one clear purpose:
- **StockQueryService**: Only read operations
- **StockValidationService**: Only validation logic
- **QuantityMoveService**: Only move operations

### 2. Small Parameter Lists
Methods accept maximum 3-4 parameters, using request DTOs for complex operations:
- ✅ `GetQuantityAtLocationAsync(itemCode, lotNumber, locationCode, warehouseCode)`
- ✅ `ValidateStockForMoveAsync(StockMoveValidationRequest request)`

### 3. Composition Over Configuration
Complex operations are built by composing smaller methods:
- `MoveQuantityWithValidationAsync` = `ValidateMoveAsync` + `MoveQuantityAsync`
- `MoveQuantityWithFifoCheckAsync` = `ValidateFifoComplianceAsync` + `MoveQuantityWithValidationAsync`

### 4. Focused Services
Services are separated by domain concern:
- **Stock Services**: Inventory queries and validation
- **Quantity Services**: Movement operations
- **FIFO Service**: FIFO-specific logic
- **Barcode Service**: Barcode parsing

### 5. Query Objects Pattern
Request DTOs replace long parameter lists:
```csharp
// Instead of:
Task<Response> Method(string p1, string p2, string p3, string p4, bool p5, int p6);

// We use:
Task<Response> Method(GetLocationsRequest request);
```

### 6. Strategy Pattern
Different strategies for different scenarios:
- Simple move: `MoveQuantityAsync`
- Move with validation: `MoveQuantityWithValidationAsync`
- Move with FIFO: `MoveQuantityWithFifoCheckAsync`

---

## Service Explanations

### StockQueryService
**Purpose**: Read-only operations for stock information.

**Methods**:
- `GetQuantityAtLocationAsync`: Returns quantity at a specific location
- `GetLocationsForItemLotAsync`: Lists all locations for an item/lot combination
- `GetStockSummaryAsync`: Provides summary statistics for an item
- `GetCurrentLocationAsync`: Finds current location(s) where item/lot exists

**Use Cases**: Stock inquiries, location lookups, inventory reports

---

### StockValidationService
**Purpose**: Validation logic for stock operations.

**Methods**:
- `ValidateItemAsync`: Checks if item exists and is lot-tracked
- `ValidateLotAsync`: Verifies lot number exists
- `ValidateLocationAsync`: Validates location exists and returns type
- `ValidateStockAvailabilityAsync`: Checks if sufficient quantity available
- `ValidateStockForMoveAsync`: Combined validation for move operations

**Use Cases**: Pre-operation validation, error prevention

---

### StockLocationService
**Purpose**: Location-specific queries.

**Methods**:
- `GetLocationsWithStockAsync`: Returns locations with available stock
- `GetLocationsWithoutStockAsync`: Returns locations where item/lot exists but qty = 0
- `GetLocationDetailsAsync`: Returns detailed location information

**Use Cases**: Location selection, location validation, stock distribution analysis

---

### QuantityMoveService
**Purpose**: Quantity movement operations.

**Methods**:
- `MoveQuantityAsync`: Basic move operation (calls stored procedure)
- `MoveQuantityWithValidationAsync`: Move with pre-validation
- `MoveQuantityWithFifoCheckAsync`: Move with FIFO compliance check

**Use Cases**: Inventory transfers, location moves, production movements

---

### QuantityValidationService
**Purpose**: Pre-move validation.

**Methods**:
- `ValidateSourceStockAsync`: Ensures source has sufficient quantity
- `ValidateTargetLocationAsync`: Ensures target location is valid
- `ValidateMoveAsync`: Combined source and target validation

**Use Cases**: Pre-move checks, error prevention

---

### FifoService
**Purpose**: FIFO (First-In-First-Out) operations.

**Methods**:
- `GetOldestLotAsync`: Retrieves oldest lot for an item
- `ValidateFifoComplianceAsync`: Checks if current lot violates FIFO
- `GetFifoSummaryAsync`: Returns FIFO summary for an item

**Use Cases**: FIFO compliance, lot selection, inventory aging

---

### BarcodeService
**Purpose**: Barcode parsing and lookup.

**Methods**:
- `ParseBarcodeAsync`: Parses barcode string (item%lot%quantity format)
- `LookupBarcodeAsync`: Looks up item information from barcode

**Use Cases**: Mobile scanning, barcode input processing

---

### StockOperationService (Composition)
**Purpose**: Combines multiple services for complex workflows.

**Methods**:
- `MoveQuantityWithFullValidationAsync`: Complete validation + move operation

**Use Cases**: High-level operations requiring multiple validations

---

## Request/Response Models

### Key Request Models

**GetQuantityRequest**
```json
{
  "item_code": "ABC123",
  "lot_number": "LOT001",
  "location_code": "LOC001",
  "warehouse_code": "WHSE01"
}
```

**MoveQuantityRequest**
```json
{
  "item_code": "ABC123",
  "source_location": "LOC001",
  "source_lot_number": "LOT001",
  "target_location": "LOC002",
  "quantity": 100.50,
  "warehouse_code": "WHSE01",
  "site_reference": "SITE01"
}
```

**StockMoveValidationRequest**
```json
{
  "item_code": "ABC123",
  "lot_number": "LOT001",
  "source_location": "LOC001",
  "target_location": "LOC002",
  "quantity": 100.50,
  "warehouse_code": "WHSE01",
  "site_reference": "SITE01"
}
```

### Key Response Models

**CombinedValidationResponse**
```json
{
  "is_valid": true,
  "item_validation": { "is_valid": true, "is_lot_tracked": true },
  "lot_validation": { "is_valid": true },
  "source_location_validation": { "is_valid": true },
  "target_location_validation": { "is_valid": true },
  "stock_availability": { "is_available": true, "available_quantity": 150.00 }
}
```

**MoveQuantityResponse**
```json
{
  "success": true,
  "transaction_id": 12345,
  "return_code": 0,
  "error_message": null
}
```

---

## Database Tables

### Core Tables

- **lot_loc_mst**: Lot location inventory (quantities by location)
- **item_mst**: Item master data (item definitions, lot tracking flag)
- **location_mst**: Location master (location definitions, types)
- **lot_mst**: Lot master (lot definitions, creation dates)
- **fifo_summary**: FIFO summary (oldest lots per item/warehouse)

### Stored Procedures

- **validate_stock**: Validates stock availability and lot tracking
- **move_quantity**: Performs quantity movement between locations

---

## Error Handling

All endpoints return standardized `ApiResponse<T>` format:

**Success Response**:
```json
{
  "success": true,
  "data": { ... },
  "message": "Operation completed successfully"
}
```

**Error Response**:
```json
{
  "success": false,
  "data": null,
  "message": "Error description",
  "errors": ["Error detail 1", "Error detail 2"]
}
```

---

## Authentication

All endpoints require JWT authentication via `[Authorize]` attribute.

**Authentication Flow**:
1. Client requests token from `/api/auth/login`
2. Server returns JWT token
3. Client includes token in `Authorization: Bearer <token>` header
4. Server validates token on each request

---

## Summary

The Quantity Move API provides a **modular, focused architecture** with:

- **24 API endpoints** across 4 controllers
- **7 service interfaces** with clear responsibilities
- **30+ request/response models** with focused parameters
- **Composition patterns** for complex operations
- **Comprehensive validation** at multiple levels
- **FIFO compliance** support
- **Barcode integration** for mobile scanning

The architecture emphasizes:
- ✅ Single Responsibility
- ✅ Small Method Signatures
- ✅ Composition Over Configuration
- ✅ Clear Separation of Concerns
- ✅ Easy Testing and Maintenance

