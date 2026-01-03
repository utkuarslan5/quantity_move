# Database Integration Documentation

## Overview

This document describes the database schema, tables, columns, and stored procedures used by the legacy application. Use this as a reference when connecting to the production database to ensure the new API is fully compatible.

**Production Database**: `SL10P`  
**Production Server**: `10.0.100.97` (or `localhost` for local testing)

---

## Table Structures

### 1. `lot_loc_mst` (Lot Location Inventory)

**Primary Table**: Stores inventory quantities by item, lot, location, and warehouse.

**Columns**:
- `item` NVARCHAR(50) NOT NULL - Item code
- `loc` NVARCHAR(50) NOT NULL - Location code
- `lot` NVARCHAR(50) NOT NULL - Lot number
- `qty_on_hand` DECIMAL(18, 3) NOT NULL - Quantity on hand
- `whse` NVARCHAR(50) NOT NULL - Warehouse code
- `site_ref` NVARCHAR(50) NULL - Site reference (optional)

**Primary Key**: `(item, loc, lot, whse)`

**Indexes**:
- Index on `item`
- Index on `loc`
- Index on `lot`
- Index on `whse`

**Note**: Legacy app sometimes uses `lot_loc` (without `_mst`) - verify if this is a view or alias.

**Usage in Legacy App**:
```sql
-- Get quantity at location
SELECT qty_on_hand FROM lot_loc_mst 
WHERE item = @item AND loc = @loc AND lot = @lot AND whse = @whse

-- Get locations with stock
SELECT loc FROM lot_loc_mst 
WHERE item = @item AND lot = @lot AND qty_on_hand > 0 AND whse = @whse

-- Verify move success
SELECT * FROM lot_loc_mst 
WHERE item = @item AND lot = @lot AND loc = @target_loc AND qty_on_hand >= @qty
```

---

### 2. `item_mst` (Item Master)

**Primary Table**: Stores item master data including lot tracking flag.

**Columns**:
- `item` NVARCHAR(50) NOT NULL PRIMARY KEY - Item code
- `lot_tracked` BIT NOT NULL - Lot tracking flag (1 = tracked, 0 = not tracked)
- `description` NVARCHAR(255) NULL - Item description
- `uom` NVARCHAR(10) NULL - Unit of measure (optional)

**Usage in Legacy App**:
```sql
-- Check if item is lot-tracked
SELECT * FROM item_mst 
WHERE lot_tracked <> 1 AND item = @item
```

**Note**: Legacy app sometimes uses `item` (without `_mst`) - verify if this is a view or alias.

---

### 3. `location_mst` (Location Master)

**Primary Table**: Stores location definitions and types.

**Columns**:
- `loc` NVARCHAR(50) NOT NULL PRIMARY KEY - Location code
- `uf_yer_tipi` NVARCHAR(10) NULL - Location type
  - Values: `'RAF'`, `'ARAF'`, `'URT'`, `'SEVK'`, `'GKK'`, `'KK'`, `'ONAY'`
- `description` NVARCHAR(255) NULL - Location description

**Location Types**:
- `RAF` - Rack location
- `ARAF` - Rack location variant
- `URT` - Production location
- `SEVK` - Shipping location
- `GKK` - Quality control location (blocked)
- `KK` - Quality control
- `ONAY` - Approved location

**Usage in Legacy App**:
```sql
-- Get location type
SELECT uf_yer_tipi FROM location_mst WHERE loc = @loc

-- Validate location exists
SELECT loc FROM location_mst WHERE loc = @loc
```

**Note**: Legacy app sometimes uses `location` (without `_mst`) - verify if this is a view or alias.

---

### 4. `lot_mst` (Lot Master)

**Primary Table**: Stores lot definitions and creation dates.

**Columns**:
- `item` NVARCHAR(50) NOT NULL - Item code
- `lot` NVARCHAR(50) NOT NULL - Lot number
- `create_date` DATETIME NULL - Creation date
- `RecordDate` DATETIME NULL - Record date (fallback if create_date is null)

**Primary Key**: `(item, lot)`

**Foreign Key**: `item` references `item_mst(item)`

**Usage in Legacy App**:
```sql
-- Get FIFO date (uses create_date or RecordDate as fallback)
SELECT isnull(create_date, RecordDate) as FIFO 
FROM lot_mst 
WHERE item = @item AND lot = @lot
```

---

### 5. `TRM_FIFO_SUM` (FIFO Summary)

**Primary Table**: Stores FIFO summary data for lot selection.

**Columns**:
- `item` NVARCHAR(50) NOT NULL - Item code
- `lot` NVARCHAR(50) NOT NULL - Lot number
- `loc` NVARCHAR(50) NOT NULL - Location code
- `qty_on_hand` DECIMAL(18, 3) NOT NULL - Quantity on hand
- `whse` NVARCHAR(50) NOT NULL - Warehouse code
- `FIFO` DATETIME NULL - **FIFO date (uppercase column name)**
- `site_ref` NVARCHAR(50) NULL - Site reference (optional)

**Primary Key**: `(item, lot, loc, whse)`

**Indexes**:
- Index on `(item, whse)`
- Index on `FIFO`

**⚠️ CRITICAL**: Column name is `FIFO` (uppercase), NOT `fifo_date`!

**Usage in Legacy App**:
```sql
-- Get oldest lot
SELECT TOP 1 lot FROM TRM_FIFO_SUM 
WHERE item = @item AND whse = @whse 
ORDER BY FIFO

-- Get FIFO summary
SELECT TOP 1 item, loc, lot, qty_on_hand, FIFO 
FROM TRM_FIFO_SUM 
WHERE item = @item AND whse = @whse
```

---

### 6. `TRM_EDIUSER` (User Authentication)

**Primary Table**: Stores user credentials and default warehouse.

**Columns**:
- `Kullanici` NVARCHAR(50) NOT NULL PRIMARY KEY - Username (Turkish: "User")
- `Sifre` NVARCHAR(50) NOT NULL - Password (Turkish: "Password") - **Plain text, no hashing**
- `Ambar` NVARCHAR(50) NOT NULL - Default warehouse (Turkish: "Warehouse")

**Usage in Legacy App**:
```sql
-- User authentication
SELECT * FROM TRM_EDIUSER 
WHERE Kullanici = @username AND Sifre = @password
```

**Security Note**: Passwords are stored in plain text (legacy behavior).

---

### 7. `employee_mst` (Employee Master)

**Primary Table**: Stores employee information for validation.

**Columns**:
- `emp_num` NVARCHAR(50) NOT NULL - Employee number
- `site_Ref` NVARCHAR(50) NOT NULL - Site reference
- `dept` NVARCHAR(50) NULL - Department
- `full_name` NVARCHAR(255) NULL - Full name

**Primary Key**: `(emp_num, site_Ref)`

**Usage in Legacy App**:
```sql
-- Validate employee exists
SELECT * FROM employee_mst 
WHERE LTRIM(emp_num) = @emp_num AND site_Ref = @site_ref

-- Get department
SELECT dept FROM employee_mst 
WHERE rtrim(ltrim(emp_num)) = @emp_num
```

**Note**: Legacy app uses `LTRIM()` to handle whitespace in employee numbers.

---

### 8. `dcitem_mst` (Transaction Master)

**Primary Table**: Stores transaction numbers and history.

**Columns**:
- `trans_num` INT NOT NULL PRIMARY KEY - Transaction number
- `trans_date` DATETIME NULL - Transaction date
- `item` NVARCHAR(50) NULL - Item code
- `lot` NVARCHAR(50) NULL - Lot number
- `loc` NVARCHAR(50) NULL - Location code
- `qty` DECIMAL(18, 3) NULL - Quantity
- `whse` NVARCHAR(50) NULL - Warehouse code

**Usage in Legacy App**:
```sql
-- Get next transaction number
SELECT isnull(max(trans_num), 0) + 1 FROM dcitem_mst
```

---

## Additional Tables (Used by Legacy but Not in New API)

These tables are used by the legacy app but are not currently required for basic quantity move operations:

### `TRM_LABELDB` (Label/Barcode Database)
- Used for barcode scanning and label lookup
- Columns: `SeriNo`, `MALZEMEKODU`, `LOT_NO`, `MIKTAR`, `TARIH`, `durum`, `Cekme`

### `TRM_TRCEKLIST` (Picking List)
- Used for picking list operations
- Columns: `CEKLIST`, `item`, etc.

### `TRM_TRCEKLIST_V` (Picking List View)
- View for picking list data
- Column: `SEVKYERI`

### `matltran` (Material Transactions)
- Used for transaction history
- Columns: `loc`, `trans_type`, `ref_type`, `qty`, `trans_date`, `item`, `whse`, `mrb_flag`

### `itemwhse` (Item Warehouse)
- Item-warehouse mapping
- Columns: `item`, `whse`

### `whse` (Warehouse Master)
- Warehouse definitions
- Column: `whse`

---

## Stored Procedures

### 1. `TR_Miktar_Ilerlet` (Move Quantity - Basic)

**Purpose**: Moves quantity from source location to target location.

**Parameters**:
- `@Item` NVARCHAR(50) - Item code
- `@loc1` NVARCHAR(50) - Source location
- `@lot1` NVARCHAR(50) - Source lot number
- `@loc2` NVARCHAR(50) - Target location
- `@qty` DECIMAL(18, 3) - Quantity to move
- `@DocumentNum` NVARCHAR(50) = NULL - Document number (optional)

**Returns**: 
- May return via SELECT or output parameters:
  - `transaction_id` BIGINT
  - `return_code` INT (0 = success, -1 = error)
  - `error_message` NVARCHAR(500)

**Usage in Legacy App**:
```sql
EXEC dbo.TR_Miktar_Ilerlet 
    @Item = 'ITEM001', 
    @loc1 = 'LOC001', 
    @lot1 = 'LOT001', 
    @loc2 = 'LOC002', 
    @qty = 100.0,
    @DocumentNum = 'DOC123'
```

**Behavior**:
1. Validates source has sufficient quantity
2. Decreases source quantity
3. Increases target quantity (or inserts if doesn't exist)
4. Records transaction in `dcitem_mst`
5. Returns transaction ID and status

---

### 2. `TR_Stok_Kontrol` (Stock Validation)

**Purpose**: Validates stock availability and lot tracking.

**Parameters**:
- `@Item` NVARCHAR(50) - Item code
- `@Lot` NVARCHAR(50) - Lot number
- `@Location` NVARCHAR(50) - Location code
- `@Whse` NVARCHAR(50) - Warehouse code

**Returns**: 
- Returns `1 AS IsValid` on success
- Raises error on failure

**Usage in Legacy App**:
```sql
EXEC TR_Stok_Kontrol 
    'ITEM001', 
    'LOT001', 
    'LOC001', 
    'MAIN'
```

**Behavior**:
1. Validates item exists and is lot-tracked
2. Validates lot exists
3. Validates location exists
4. Returns success or raises error

---

## Additional Stored Procedure Variants

The legacy app uses several variants of the stored procedures. These are **not currently implemented** in the new API but may be needed:

### `TR_Miktar_Ilerlet_MI` (Production Move)
**Additional Parameters**:
- `@whse` NVARCHAR(50) - Warehouse
- `@UserName` NVARCHAR(50) - User name
- `@Sicil` NVARCHAR(50) - Employee number

### `TR_Miktar_Ilerlet_MultiWhse` (Multi-Warehouse Transfer)
**Additional Parameters**:
- `@whse1` NVARCHAR(50) - Source warehouse
- `@whse2` NVARCHAR(50) - Target warehouse
- `@UserName` NVARCHAR(50) - User name

### `TRM_Miktar_Ilerlet` (Variant)
- Similar to `TR_Miktar_Ilerlet` but may have different behavior

### `TRM_Stok_Kontrol` (Variant)
- Similar to `TR_Stok_Kontrol` but may have different behavior

### `TRM_Stok_Kontrol2` (Variant)
- Enhanced version with additional validation

### `TRM_Dcitem_Insert_Post` (Transaction Posting)
- Used for posting transactions
- Parameters: `@trans_num`, `@trans_date`, `@item`, `@lot`, `@loc`, `@qty`, `@whse`, `@siteref`, `@count_qty`

---

## Table Name Variations

**⚠️ IMPORTANT**: The legacy app uses both full table names and shortened versions:

| Full Name | Short Name Used | Status |
|-----------|----------------|--------|
| `lot_loc_mst` | `lot_loc` | **Verify if view/alias exists** |
| `location_mst` | `location` | **Verify if view/alias exists** |
| `item_mst` | `item` | **Verify if view/alias exists** |

**Action Required**: When connecting to production database, verify:
1. Do views `lot_loc`, `location`, `item` exist?
2. Or are these just inconsistent naming in legacy code?
3. If views exist, ensure they point to the `_mst` tables

---

## Column Name Reference

### Core Columns (All Tables)
- `item` - Item code
- `lot` - Lot number
- `loc` - Location code
- `whse` - Warehouse code
- `qty_on_hand` - Quantity on hand
- `site_ref` - Site reference

### Item Master
- `lot_tracked` - Lot tracking flag (BIT)

### Location Master
- `uf_yer_tipi` - Location type

### Lot Master
- `create_date` - Creation date
- `RecordDate` - Record date (fallback)

### FIFO Summary
- `FIFO` - **FIFO date (uppercase, NOT fifo_date)**

### User Table
- `Kullanici` - Username
- `Sifre` - Password
- `Ambar` - Warehouse

### Employee Table
- `emp_num` - Employee number
- `site_Ref` - Site reference
- `dept` - Department
- `full_name` - Full name

---

## Data Type Reference

| Column Type | SQL Server Type | Notes |
|------------|----------------|-------|
| Item/Lot/Loc codes | `NVARCHAR(50)` | Unicode strings |
| Quantities | `DECIMAL(18, 3)` | 18 digits, 3 decimal places |
| Dates | `DATETIME` | Date and time |
| Flags | `BIT` | Boolean (0/1) |
| Warehouse | `NVARCHAR(50)` | Unicode string |

---

## Query Patterns from Legacy App

### Stock Inquiry with FIFO
```sql
SELECT item, loc, lot, cast(qty_on_hand as decimal(18,2)) qty_on_hand, 
  (SELECT isnull(create_date, RecordDate) as FIFO 
   FROM lot_mst lot 
   WHERE lot.item = lot_loc.item AND lot.lot = lot_loc.lot) AS fifo
FROM lot_loc_mst lot_loc 
WHERE qty_on_hand > 0 AND item = @item
ORDER BY item, fifo
```

### Location Lookup with Stock
```sql
SELECT (loc + '-' + cast(cast(qty_on_hand as decimal(10,2)) as nvarchar(18))) as loc  
FROM lot_loc_mst 
WHERE item = @item AND lot = @lot AND qty_on_hand > 0 AND whse = @whse
```

### Quantity Validation
```sql
SELECT TOP 1 qty_on_hand 
FROM lot_loc_mst 
WHERE item = @item AND loc = @loc AND lot = @lot AND whse = @whse
```

---

## Verification Checklist

When connecting to production database, verify:

### Tables
- [ ] `lot_loc_mst` exists with all columns
- [ ] `item_mst` exists with `lot_tracked` column
- [ ] `location_mst` exists with `uf_yer_tipi` column
- [ ] `lot_mst` exists with `create_date` and `RecordDate`
- [ ] `TRM_FIFO_SUM` exists with `FIFO` column (uppercase)
- [ ] `TRM_EDIUSER` exists with `Kullanici`, `Sifre`, `Ambar`
- [ ] `employee_mst` exists with `emp_num`, `site_Ref`
- [ ] `dcitem_mst` exists for transaction numbers
- [ ] Views `lot_loc`, `location`, `item` exist (if used)

### Stored Procedures
- [ ] `TR_Miktar_Ilerlet` exists with correct parameters
- [ ] `TR_Stok_Kontrol` exists with correct parameters
- [ ] Stored procedures return values via output parameters OR result set
- [ ] Verify return value format (output params vs SELECT)

### Column Names
- [ ] `FIFO` column in `TRM_FIFO_SUM` is uppercase (not `fifo_date`)
- [ ] All column names match exactly (case-sensitive)
- [ ] `qty_on_hand` is `DECIMAL(18, 3)`
- [ ] `lot_tracked` is `BIT` type

### Data
- [ ] Sample data exists for testing
- [ ] User credentials in `TRM_EDIUSER` for testing
- [ ] Employee records in `employee_mst` for testing

---

## Known Issues & Fixes Required

### 1. FIFO Column Name (CRITICAL)
**Issue**: New API uses `fifo_date` but database uses `FIFO` (uppercase)

**Fix**: Update `backend/Common/Constants/ColumnNames.cs`:
```csharp
public const string FifoDate = "FIFO";  // Change from "fifo_date"
```

### 2. Table Name Variations
**Issue**: Legacy app uses both `lot_loc` and `lot_loc_mst`

**Action**: Verify in production if views exist or if legacy code is inconsistent

### 3. Stored Procedure Return Values
**Issue**: Need to verify if SPs return via output parameters or result set

**Action**: Test stored procedure execution and verify return mechanism

---

## Connection String Format

**Production**:
```
Data Source=10.0.100.97;Initial Catalog=SL10P;User ID=sa2;pwd=;Connect Timeout=0;Encrypt=True;TrustServerCertificate=True
```

**Local/Test**:
```
Data Source=localhost;Initial Catalog=SL10P;User ID=sa2;pwd=;Connect Timeout=0;Encrypt=True;TrustServerCertificate=True
```

---

## Testing Queries

Use these queries to verify database connectivity and structure:

```sql
-- Verify tables exist
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_SCHEMA = 'dbo' 
  AND TABLE_NAME IN ('lot_loc_mst', 'item_mst', 'location_mst', 'lot_mst', 'TRM_FIFO_SUM', 'TRM_EDIUSER', 'employee_mst', 'dcitem_mst')

-- Verify FIFO column name
SELECT COLUMN_NAME 
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'TRM_FIFO_SUM' 
  AND COLUMN_NAME LIKE '%FIFO%'

-- Verify stored procedures exist
SELECT ROUTINE_NAME 
FROM INFORMATION_SCHEMA.ROUTINES 
WHERE ROUTINE_SCHEMA = 'dbo' 
  AND ROUTINE_NAME IN ('TR_Miktar_Ilerlet', 'TR_Stok_Kontrol')

-- Check for views
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.VIEWS 
WHERE TABLE_SCHEMA = 'dbo' 
  AND TABLE_NAME IN ('lot_loc', 'location', 'item')
```

---

## Notes

1. **Password Storage**: Legacy app stores passwords in plain text - maintain this for compatibility
2. **Transaction Numbers**: Generated from `dcitem_mst` using `MAX(trans_num) + 1`
3. **Site Reference**: Default site is `"faz"` if not specified
4. **Warehouse**: Default warehouse is `"MAIN"`
5. **Location Types**: Special handling for `GKK` (blocked) and `KK` (quality control)
6. **FIFO Calculation**: Uses `ISNULL(create_date, RecordDate)` from `lot_mst` table

---

## API Configuration Mapping

Current API configuration in `appsettings.json`:

```json
{
  "Database": {
    "Tables": {
      "LotLocation": "lot_loc_mst",
      "ItemMaster": "item_mst",
      "LocationMaster": "location_mst",
      "LotMaster": "lot_mst",
      "UserMaster": "TRM_EDIUSER",
      "FifoSummary": "TRM_FIFO_SUM"
    }
  },
  "StoredProcedures": {
    "ValidateStock": "TR_Stok_Kontrol",
    "MoveQuantity": "TR_Miktar_Ilerlet"
  }
}
```

**Status**: ✅ All table and procedure names are correctly configured.

---

## Next Steps

1. **Fix FIFO Column Name**: Update `ColumnNames.cs` to use `FIFO` instead of `fifo_date`
2. **Verify Table Names**: Check if views `lot_loc`, `location`, `item` exist in production
3. **Test Stored Procedures**: Execute `TR_Miktar_Ilerlet` and verify return mechanism
4. **Test Authentication**: Verify `TRM_EDIUSER` table structure matches expectations
5. **Test FIFO Queries**: Verify `TRM_FIFO_SUM` queries work with `FIFO` column name

---

**Last Updated**: Based on legacy app analysis  
**Database**: SL10P  
**Legacy App Version**: MobileMiktarIlerlet

