# Getting Started with Quantity Move API

## Table of Contents
1. [Quick Start](#quick-start)
2. [Authentication](#authentication)
3. [Business Logic Guide](#business-logic-guide)
4. [Endpoint Reference](#endpoint-reference)
5. [Request/Response Formats](#requestresponse-formats)
6. [Common Workflows](#common-workflows)
7. [Error Handling](#error-handling)

---

## Quick Start

### 1. Base URL
All API endpoints are prefixed with `/api`:
```
https://your-server.com/api
```

### 2. Authentication
Most endpoints require JWT authentication. Get your token first (see [Authentication](#authentication) section).

### 3. Default Values
The API uses default values from configuration:
- **Default Warehouse**: `MAIN` (configurable in `appsettings.json`)
- **Default Site**: `Default` (configurable in `appsettings.json`)

If you don't provide `warehouse_code` or `site_reference` in requests, these defaults will be used automatically.

---

## Authentication

### Login Endpoint
**POST** `/api/auth/login`

**Request Body:**
```json
{
  "username": "your_username",
  "password": "your_password"
}
```

**Response:**
```json
{
  "success": true,
  "message": "Login successful",
  "data": {
    "token": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
    "expires_in": 86400,
    "user": {
      "user_id": 123,
      "username": "your_username",
      "full_name": "Your Full Name",
      "email": "your.email@example.com"
    }
  }
}
```

**What it does:**
- Validates username and password against the database
- Looks up user in `user_mst`, `employees`, or `users` tables
- Returns JWT token valid for 24 hours (configurable)

**Usage:**
```bash
curl -X POST https://your-server.com/api/auth/login \
  -H "Content-Type: application/json" \
  -d '{"username":"your_username","password":"your_password"}'
```

### Using the Token
Include the token in the `Authorization` header for all protected endpoints:
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
```

---

## Business Logic Guide

### Scenario 1: Check Stock Quantity at a Location

**What you want to do:** Find out how much of an item is available at a specific location.

**Endpoint to use:** `GET /api/stock/quantity`

**What it looks up:**
- Queries `lot_loc_mst` table
- Finds quantity on hand for: item + lot + location + warehouse

**Request:**
```
GET /api/stock/quantity?item_code=ITEM123&lot_number=LOT001&location_code=LOC-A1&warehouse_code=MAIN
```

**Response:**
```json
{
  "success": true,
  "data": 150.50,
  "message": null
}
```

**Example:**
```bash
curl -X GET "https://your-server.com/api/stock/quantity?item_code=ITEM123&lot_number=LOT001&location_code=LOC-A1" \
  -H "Authorization: Bearer YOUR_TOKEN"
```

---

### Scenario 2: Find All Locations Where an Item/Lot is Stored

**What you want to do:** Get a list of all locations that have stock for a specific item and lot.

**Endpoint to use:** `GET /api/stock/locations`

**What it looks up:**
- Queries `lot_loc_mst` table
- Finds all locations with stock for: item + lot + warehouse
- Optionally includes locations with zero quantity

**Request:**
```
GET /api/stock/locations?item_code=ITEM123&lot_number=LOT001&warehouse_code=MAIN&include_zero_quantity=false
```

**Response:**
```json
{
  "success": true,
  "data": {
    "locations": [
      {
        "location_code": "LOC-A1",
        "quantity_on_hand": 150.50,
        "location_display": "LOC-A1-150.50"
      },
      {
        "location_code": "LOC-B2",
        "quantity_on_hand": 75.25,
        "location_display": "LOC-B2-75.25"
      }
    ]
  }
}
```

---

### Scenario 3: Validate Before Moving Stock

**What you want to do:** Check if you can move stock from one location to another before actually doing it.

**Recommended workflow:**
1. Validate item exists and is lot-tracked
2. Validate lot exists
3. Validate source location has enough stock
4. Validate target location is valid
5. (Optional) Check FIFO compliance
6. Perform the move

**Endpoints to use:**

#### Step 1: Validate Item
**POST** `/api/stock/validate/item`

**What it looks up:**
- Queries `item_mst` table
- Checks if item exists
- Verifies if item is lot-tracked

**Request:**
```json
{
  "item_code": "ITEM123",
  "site_reference": "Default"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "is_valid": true,
    "item_code": "ITEM123",
    "is_lot_tracked": true,
    "description": "Item Description",
    "error_message": null
  }
}
```

#### Step 2: Validate Lot
**POST** `/api/stock/validate/lot`

**What it looks up:**
- Queries `lot_mst` table
- Checks if lot exists for the item

**Request:**
```json
{
  "item_code": "ITEM123",
  "lot_number": "LOT001"
}
```

#### Step 3: Validate Stock Availability
**POST** `/api/stock/validate/availability`

**What it looks up:**
- Queries `lot_loc_mst` table
- Checks available quantity at source location
- Compares with required quantity

**Request:**
```json
{
  "item_code": "ITEM123",
  "lot_number": "LOT001",
  "location_code": "LOC-A1",
  "required_quantity": 100.0,
  "warehouse_code": "MAIN"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "is_available": true,
    "item_code": "ITEM123",
    "lot_number": "LOT001",
    "location_code": "LOC-A1",
    "available_quantity": 150.50,
    "required_quantity": 100.0,
    "error_message": null
  }
}
```

#### Step 4: Validate Target Location
**POST** `/api/stock/validate/location`

**What it looks up:**
- Queries `location_mst` table
- Checks if location exists
- Gets location type and description

**Request:**
```json
{
  "location_code": "LOC-B2",
  "site_reference": "Default"
}
```

#### Step 5: Combined Validation (Recommended)
**POST** `/api/stock/validate/move`

**What it does:**
- Performs all validations above in one call
- Returns detailed validation results for each step

**Request:**
```json
{
  "item_code": "ITEM123",
  "lot_number": "LOT001",
  "source_location": "LOC-A1",
  "target_location": "LOC-B2",
  "quantity": 100.0,
  "warehouse_code": "MAIN",
  "site_reference": "Default"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "is_valid": true,
    "item_validation": {
      "is_valid": true,
      "item_code": "ITEM123",
      "is_lot_tracked": true
    },
    "lot_validation": {
      "is_valid": true,
      "lot_number": "LOT001"
    },
    "source_location_validation": {
      "is_valid": true,
      "location_code": "LOC-A1"
    },
    "target_location_validation": {
      "is_valid": true,
      "location_code": "LOC-B2"
    },
    "stock_availability": {
      "is_available": true,
      "available_quantity": 150.50,
      "required_quantity": 100.0
    },
    "error_message": null
  }
}
```

---

### Scenario 4: Move Stock from One Location to Another

**What you want to do:** Transfer quantity from source location to target location.

**Endpoint to use:** `POST /api/quantity/move`

**What it does:**
- Executes stored procedure `move_quantity`
- Updates `lot_loc_mst` table (decreases source, increases target)
- Creates transaction record
- Returns transaction ID

**Request:**
```json
{
  "item_code": "ITEM123",
  "source_location": "LOC-A1",
  "source_lot_number": "LOT001",
  "target_location": "LOC-B2",
  "quantity": 100.0,
  "warehouse_code": "MAIN",
  "site_reference": "Default"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "success": true,
    "transaction_id": 12345,
    "return_code": 0,
    "error_message": null
  },
  "message": "Move operation completed successfully"
}
```

**Important:** Always validate before moving! Use the validation endpoints first.

---

### Scenario 5: Move with Automatic Validation

**What you want to do:** Move stock with built-in validation (recommended).

**Endpoint to use:** `POST /api/quantity/move-with-validation`

**What it does:**
- Validates source stock availability
- Validates target location
- Performs the move if validation passes
- Returns error if validation fails

**Request:** Same as `/api/quantity/move`

**Response:** Same format, but will return error if validation fails:
```json
{
  "success": true,
  "data": {
    "success": false,
    "transaction_id": null,
    "return_code": -1,
    "error_message": "Insufficient stock at source location. Available: 50.0, Required: 100.0"
  }
}
```

---

### Scenario 6: Check FIFO Compliance

**What you want to do:** Verify if you're moving the oldest lot first (First-In-First-Out).

**Endpoint to use:** `POST /api/fifo/validate`

**What it looks up:**
- Queries `fifo_summary` table
- Finds oldest lot by FIFO date
- Compares with current lot being moved

**Request:**
```json
{
  "item_code": "ITEM123",
  "lot_number": "LOT001",
  "warehouse_code": "MAIN",
  "site_reference": "Default"
}
```

**Response:**
```json
{
  "success": true,
  "data": {
    "is_compliant": true,
    "item_code": "ITEM123",
    "current_lot_number": "LOT001",
    "oldest_lot_number": "LOT001",
    "warning_message": null
  }
}
```

**If not compliant:**
```json
{
  "success": true,
  "data": {
    "is_compliant": false,
    "item_code": "ITEM123",
    "current_lot_number": "LOT002",
    "oldest_lot_number": "LOT001",
    "warning_message": "Older lot exists: LOT001 at location LOC-A1 (FIFO date: 2024-01-15)"
  }
}
```

---

### Scenario 7: Get Oldest Lot for FIFO

**What you want to do:** Find which lot should be moved first according to FIFO.

**Endpoint to use:** `GET /api/fifo/oldest-lot`

**What it looks up:**
- Queries `fifo_summary` table
- Orders by FIFO date (oldest first)
- Returns location and quantity of oldest lot

**Request:**
```
GET /api/fifo/oldest-lot?item_code=ITEM123&warehouse_code=MAIN&site_reference=Default
```

**Response:**
```json
{
  "success": true,
  "data": {
    "item_code": "ITEM123",
    "location_code": "LOC-A1",
    "oldest_lot_number": "LOT001",
    "quantity_on_hand": 150.50,
    "fifo_date": "2024-01-15T00:00:00"
  }
}
```

---

### Scenario 8: Parse Barcode

**What you want to do:** Extract item code, lot number, and quantity from a barcode string.

**Endpoint to use:** `POST /api/barcode/parse`

**What it does:**
- Parses barcode format: `item%lot%quantity`
- Extracts components
- Does NOT query database (just parsing)

**Request:**
```json
"ITEM123%LOT001%100.50"
```

**Response:**
```json
{
  "success": true,
  "data": {
    "is_valid": true,
    "item_code": "ITEM123",
    "lot_number": "LOT001",
    "quantity": 100.50,
    "error_message": null
  }
}
```

---

### Scenario 9: Lookup Item from Barcode

**What you want to do:** Parse barcode AND get item information from database.

**Endpoint to use:** `POST /api/barcode/lookup`

**What it does:**
- Parses barcode
- Queries `item_mst` table for item details
- Returns item description and other info

**Request:**
```json
"ITEM123%LOT001%100.50"
```

**Response:**
```json
{
  "success": true,
  "data": {
    "found": true,
    "item_code": "ITEM123",
    "lot_number": "LOT001",
    "description": "Product Description",
    "error_message": null
  }
}
```

---

### Scenario 10: Get Stock Summary

**What you want to do:** Get overview of all stock locations for an item.

**Endpoint to use:** `GET /api/stock/summary`

**What it looks up:**
- Queries `lot_loc_mst` table
- Aggregates all locations for item + warehouse
- Calculates total quantity

**Request:**
```
GET /api/stock/summary?item_code=ITEM123&warehouse_code=MAIN
```

**Response:**
```json
{
  "success": true,
  "data": {
    "item_code": "ITEM123",
    "warehouse_code": "MAIN",
    "total_quantity": 225.75,
    "location_count": 2,
    "locations": [
      {
        "location_code": "LOC-A1",
        "lot_number": "LOT001",
        "quantity_on_hand": 150.50
      },
      {
        "location_code": "LOC-B2",
        "lot_number": "LOT001",
        "quantity_on_hand": 75.25
      }
    ]
  }
}
```

---

## Endpoint Reference

### Stock Endpoints (`/api/stock`)

| Method | Endpoint | Purpose | What It Looks Up |
|--------|----------|---------|------------------|
| `GET` | `/quantity` | Get quantity at location | `lot_loc_mst` - quantity for item+lot+location+warehouse |
| `GET` | `/locations` | Get all locations for item/lot | `lot_loc_mst` - all locations with stock |
| `GET` | `/locations/with-stock` | Get locations with stock > 0 | `lot_loc_mst` - filtered by quantity > 0 |
| `GET` | `/current-location` | Get current location(s) | `lot_loc_mst` - locations for item+lot |
| `GET` | `/summary` | Get stock summary | `lot_loc_mst` - aggregated by item |
| `POST` | `/validate/item` | Validate item exists | `item_mst` - item validation |
| `POST` | `/validate/lot` | Validate lot exists | `lot_mst` - lot validation |
| `POST` | `/validate/location` | Validate location exists | `location_mst` - location validation |
| `POST` | `/validate/availability` | Check stock availability | `lot_loc_mst` - quantity check |
| `POST` | `/validate/move` | Combined validation | All above validations |

### Quantity Endpoints (`/api/quantity`)

| Method | Endpoint | Purpose | What It Looks Up |
|--------|----------|---------|------------------|
| `POST` | `/validate/source` | Validate source stock | `lot_loc_mst` - source quantity |
| `POST` | `/validate/target` | Validate target location | `location_mst` - target location |
| `POST` | `/validate/move` | Validate move operation | Both source and target |
| `POST` | `/move` | Move quantity | Executes `move_quantity` stored procedure |
| `POST` | `/move-with-validation` | Move with validation | Validates then moves |
| `POST` | `/move-with-fifo-check` | Move with FIFO check | Validates FIFO then moves |

### FIFO Endpoints (`/api/fifo`)

| Method | Endpoint | Purpose | What It Looks Up |
|--------|----------|---------|------------------|
| `GET` | `/oldest-lot` | Get oldest lot | `fifo_summary` - oldest by FIFO date |
| `POST` | `/validate` | Validate FIFO compliance | `fifo_summary` - compares lots |
| `GET` | `/summary` | Get FIFO summary | `fifo_summary` - all lots ordered by FIFO |

### Barcode Endpoints (`/api/barcode`)

| Method | Endpoint | Purpose | What It Looks Up |
|--------|----------|---------|------------------|
| `POST` | `/parse` | Parse barcode string | No DB lookup - just parsing |
| `POST` | `/lookup` | Parse and lookup item | `item_mst` - item details |

---

## Request/Response Formats

### Standard Request Format

All POST requests use JSON body:
```json
{
  "field_name": "value"
}
```

Query parameters for GET requests:
```
?param1=value1&param2=value2
```

### Standard Response Format

All responses follow this structure:
```json
{
  "success": true,
  "data": { /* response data */ },
  "message": "Optional message",
  "errors": null
}
```

**Error Response:**
```json
{
  "success": false,
  "data": null,
  "message": "Error description",
  "errors": ["Error detail 1", "Error detail 2"]
}
```

### Common Request Fields

| Field | Type | Required | Description | Default |
|-------|------|----------|-------------|---------|
| `item_code` | string | Yes | Item identifier | - |
| `lot_number` | string | Yes* | Lot identifier | - |
| `location_code` | string | Yes | Location identifier | - |
| `warehouse_code` | string | No | Warehouse code | `MAIN` |
| `site_reference` | string | No | Site reference | `Default` |
| `quantity` | decimal | Yes | Quantity amount | - |

*Required for lot-tracked items

---

## Common Workflows

### Workflow 1: Standard Stock Move

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant DB

    Client->>API: POST /api/stock/validate/move
    API->>DB: Validate item, lot, locations, stock
    DB-->>API: Validation results
    API-->>Client: Validation response
    
    alt Validation successful
        Client->>API: POST /api/quantity/move
        API->>DB: Execute move_quantity SP
        DB-->>API: Transaction ID
        API-->>Client: Success response
    else Validation failed
        API-->>Client: Error response
    end
```

**Step-by-step:**
1. Validate the move: `POST /api/stock/validate/move`
2. If valid, perform move: `POST /api/quantity/move`
3. Check response for success/error

### Workflow 2: FIFO-Compliant Move

```mermaid
sequenceDiagram
    participant Client
    participant API
    participant DB

    Client->>API: GET /api/fifo/oldest-lot
    API->>DB: Query fifo_summary
    DB-->>API: Oldest lot info
    API-->>Client: Oldest lot response
    
    Client->>API: POST /api/fifo/validate
    API->>DB: Check FIFO compliance
    DB-->>API: Compliance result
    API-->>Client: FIFO validation
    
    alt FIFO compliant
        Client->>API: POST /api/quantity/move-with-fifo-check
        API->>DB: Move quantity
        DB-->>API: Success
        API-->>Client: Move completed
    end
```

**Step-by-step:**
1. Get oldest lot: `GET /api/fifo/oldest-lot?item_code=ITEM123`
2. Validate FIFO: `POST /api/fifo/validate` (with lot you want to move)
3. If compliant, move: `POST /api/quantity/move-with-fifo-check`

### Workflow 3: Barcode Scanning Workflow

```mermaid
sequenceDiagram
    participant Scanner
    participant Client
    participant API
    participant DB

    Scanner->>Client: Barcode: "ITEM123%LOT001%100"
    Client->>API: POST /api/barcode/lookup
    API->>DB: Query item_mst
    DB-->>API: Item details
    API-->>Client: Item info + parsed barcode
    
    Client->>API: GET /api/stock/quantity
    API->>DB: Query lot_loc_mst
    DB-->>API: Current quantity
    API-->>Client: Quantity response
    
    Client->>API: POST /api/quantity/move-with-validation
    API->>DB: Validate and move
    DB-->>API: Transaction result
    API-->>Client: Move result
```

**Step-by-step:**
1. Scan barcode → Parse: `POST /api/barcode/lookup`
2. Check current stock: `GET /api/stock/quantity`
3. Move with validation: `POST /api/quantity/move-with-validation`

---

## Error Handling

### HTTP Status Codes

| Code | Meaning | When It Occurs |
|------|---------|----------------|
| `200` | Success | Request completed successfully |
| `400` | Bad Request | Invalid request parameters |
| `401` | Unauthorized | Missing or invalid JWT token |
| `404` | Not Found | Endpoint doesn't exist |
| `500` | Server Error | Database error or internal exception |

### Common Error Messages

**Authentication Error:**
```json
{
  "success": false,
  "message": "Invalid username or password",
  "data": null
}
```

**Validation Error:**
```json
{
  "success": false,
  "message": "Item not found",
  "data": {
    "is_valid": false,
    "error_message": "Item not found"
  }
}
```

**Insufficient Stock:**
```json
{
  "success": true,
  "data": {
    "is_available": false,
    "available_quantity": 50.0,
    "required_quantity": 100.0,
    "error_message": "Insufficient stock. Available: 50.0, Required: 100.0"
  }
}
```

**Move Operation Failed:**
```json
{
  "success": true,
  "data": {
    "success": false,
    "return_code": -1,
    "error_message": "Database error: Cannot move quantity"
  }
}
```

### Error Handling Best Practices

1. **Always check `success` field** in response
2. **Check `return_code`** for move operations (0 = success, non-zero = error)
3. **Read `error_message`** for detailed error information
4. **Handle 401 errors** by re-authenticating
5. **Validate before moving** to avoid database errors

---

## Tips and Best Practices

### 1. Use Default Values
If you don't specify `warehouse_code` or `site_reference`, the API uses configured defaults. This simplifies your requests.

### 2. Validate Before Moving
Always use validation endpoints before performing moves. This prevents errors and provides better user feedback.

### 3. Use Combined Validation
Instead of calling multiple validation endpoints, use `/api/stock/validate/move` for comprehensive validation in one call.

### 4. Check FIFO for Inventory Management
If your business requires FIFO compliance, always check `/api/fifo/validate` before moving stock.

### 5. Handle Errors Gracefully
Check the `success` field and `error_message` in responses. Display meaningful error messages to users.

### 6. Use Move-with-Validation Endpoints
Endpoints like `/api/quantity/move-with-validation` combine validation and move operations, reducing API calls.

### 7. Batch Operations
For multiple moves, validate all first, then perform moves sequentially. Don't skip validation to save API calls.

---

## Example: Complete Move Operation

Here's a complete example of moving stock with full validation:

```javascript
// 1. Login
const loginResponse = await fetch('/api/auth/login', {
  method: 'POST',
  headers: { 'Content-Type': 'application/json' },
  body: JSON.stringify({
    username: 'user',
    password: 'pass'
  })
});
const { data: { token } } = await loginResponse.json();

// 2. Validate move
const validateResponse = await fetch('/api/stock/validate/move', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    item_code: 'ITEM123',
    lot_number: 'LOT001',
    source_location: 'LOC-A1',
    target_location: 'LOC-B2',
    quantity: 100.0,
    warehouse_code: 'MAIN'
  })
});
const validation = await validateResponse.json();

if (!validation.data.is_valid) {
  console.error('Validation failed:', validation.data.error_message);
  return;
}

// 3. Perform move
const moveResponse = await fetch('/api/quantity/move', {
  method: 'POST',
  headers: {
    'Content-Type': 'application/json',
    'Authorization': `Bearer ${token}`
  },
  body: JSON.stringify({
    item_code: 'ITEM123',
    source_location: 'LOC-A1',
    source_lot_number: 'LOT001',
    target_location: 'LOC-B2',
    quantity: 100.0,
    warehouse_code: 'MAIN'
  })
});
const moveResult = await moveResponse.json();

if (moveResult.data.success) {
  console.log('Move successful! Transaction ID:', moveResult.data.transaction_id);
} else {
  console.error('Move failed:', moveResult.data.error_message);
}
```

---

## Need Help?

- Check the full API documentation: `API_DOCUMENTATION.md`
- Review error messages for specific issues
- Ensure your JWT token is valid and not expired
- Verify database connection and table names match your schema

