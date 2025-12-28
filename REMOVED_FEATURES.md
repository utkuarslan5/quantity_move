# Removed Features - API Simplification

This document lists all features, endpoints, controllers, and services that were removed during the API simplification. This information is preserved in case you need to reimplement any of these features in the future.

## Removed Controllers

### 1. BarcodeController (`backend/Controllers/BarcodeController.cs`)
**Route**: `api/barcode`

**Removed Endpoints**:
- `POST /api/barcode/parse` - Parse barcode string
  - Takes: `string barcode` (in request body)
  - Returns: `BarcodeParseResponse`
  - Service: `IBarcodeService.ParseBarcodeAsync()`

- `POST /api/barcode/lookup` - Lookup item/lot from barcode
  - Takes: `string barcode` (in request body)
  - Returns: `BarcodeLookupResponse`
  - Service: `IBarcodeService.LookupBarcodeAsync()`

**Note**: Barcode parsing logic is now integrated into the new stock endpoint.

---

### 2. FifoController (`backend/Controllers/FifoController.cs`)
**Route**: `api/fifo`

**Removed Endpoints**:
- `GET /api/fifo/oldest-lot` - Get oldest lot for item
  - Query params: `itemCode` (required), `warehouseCode` (optional), `siteReference` (optional)
  - Returns: `FifoOldestLotResponse`
  - Service: `IFifoService.GetOldestLotAsync()`

- `POST /api/fifo/validate` - Validate FIFO compliance
  - Takes: `FifoValidationRequest`
  - Returns: `FifoValidationResponse`
  - Service: `IFifoService.ValidateFifoComplianceAsync()`

- `GET /api/fifo/summary` - Get FIFO summary for item
  - Query params: `itemCode` (required), `warehouseCode` (optional), `siteReference` (optional)
  - Returns: `FifoSummaryResponse`
  - Service: `IFifoService.GetFifoSummaryAsync()`

---

### 3. StockController - Most Endpoints Removed (`backend/Controllers/StockController.cs`)
**Route**: `api/stock`

**Removed Endpoints** (only kept: `GET /api/stock/{barcode}`):

#### Query Endpoints:
- `GET /api/stock/quantity` - Get quantity at specific location
  - Query params: `GetQuantityRequest` (itemCode, lotNumber, locationCode, warehouseCode)
  - Returns: `decimal`
  - Service: `IStockQueryService.GetQuantityAtLocationAsync()`

- `GET /api/stock/locations` - Get all locations for item/lot
  - Query params: `GetLocationsRequest` (itemCode, lotNumber, warehouseCode, includeZeroQuantity)
  - Returns: `LocationsResponse`
  - Service: `IStockQueryService.GetLocationsForItemLotAsync()`
  - **Note**: Similar functionality now available via barcode endpoint

- `GET /api/stock/locations/with-stock` - Get locations with stock for item/lot
  - Query params: `GetLocationsRequest`
  - Returns: `LocationsWithStockResponse`
  - Service: `IStockLocationService.GetLocationsWithStockAsync()`

- `GET /api/stock/current-location` - Get current location(s) for item/lot
  - Query params: `GetCurrentLocationRequest` (itemCode, lotNumber, warehouseCode)
  - Returns: `CurrentLocationResponse`
  - Service: `IStockQueryService.GetCurrentLocationAsync()`

- `GET /api/stock/summary` - Get stock summary for item
  - Query params: `itemCode` (required), `warehouseCode` (optional)
  - Returns: `StockSummaryResponse`
  - Service: `IStockQueryService.GetStockSummaryAsync()`

#### Validation Endpoints:
- `POST /api/stock/validate/item` - Validate item exists and is lot-tracked
  - Takes: `ValidateItemRequest` (itemCode, siteReference)
  - Returns: `ItemValidationResponse`
  - Service: `IStockValidationService.ValidateItemAsync()`

- `POST /api/stock/validate/lot` - Validate lot exists
  - Takes: `ValidateLotRequest` (itemCode, lotNumber)
  - Returns: `LotValidationResponse`
  - Service: `IStockValidationService.ValidateLotAsync()`

- `POST /api/stock/validate/location` - Validate location exists and is valid
  - Takes: `ValidateLocationRequest` (locationCode, siteReference)
  - Returns: `LocationValidationResponse`
  - Service: `IStockValidationService.ValidateLocationAsync()`

- `POST /api/stock/validate/availability` - Validate stock availability at location
  - Takes: `ValidateStockRequest` (itemCode, lotNumber, locationCode, requiredQuantity, warehouseCode)
  - Returns: `StockAvailabilityResponse`
  - Service: `IStockValidationService.ValidateStockAvailabilityAsync()`

- `POST /api/stock/validate/move` - Combined validation for move operation
  - Takes: `StockMoveValidationRequest`
  - Returns: `CombinedValidationResponse`
  - Service: `IStockValidationService.ValidateStockForMoveAsync()`

---

### 4. QuantityController - Most Endpoints Removed (`backend/Controllers/QuantityController.cs`)
**Route**: `api/quantity`

**Removed Endpoints** (functionality moved to new MoveController):

#### Validation Endpoints:
- `POST /api/quantity/validate/source` - Validate source location has sufficient stock
  - Takes: `ValidateSourceRequest` (itemCode, lotNumber, sourceLocation, quantity, warehouseCode)
  - Returns: `ValidationResponse`
  - Service: `IQuantityValidationService.ValidateSourceStockAsync()`
  - **Note**: Now part of `/api/move/validate`

- `POST /api/quantity/validate/target` - Validate target location is valid for receiving
  - Takes: `ValidateTargetRequest` (itemCode, targetLocation, warehouseCode)
  - Returns: `ValidationResponse`
  - Service: `IQuantityValidationService.ValidateTargetLocationAsync()`
  - **Note**: Now part of `/api/move/validate`

- `POST /api/quantity/validate/move` - Validate move is allowed (combines source + target)
  - Takes: `MoveValidationRequest`
  - Returns: `MoveValidationResponse`
  - Service: `IQuantityValidationService.ValidateMoveAsync()`
  - **Note**: Moved to `/api/move/validate`

#### Move Endpoints:
- `POST /api/quantity/move` - Simple move (no validation)
  - Takes: `MoveQuantityRequest`
  - Returns: `MoveQuantityResponse`
  - Service: `IQuantityMoveService.MoveQuantityAsync()`
  - **Note**: Moved to `/api/move` with validation built-in

- `POST /api/quantity/move/with-validation` - Move with validation
  - Takes: `MoveQuantityRequest`
  - Returns: `MoveQuantityResponse`
  - Service: `IQuantityMoveService.MoveQuantityWithValidationAsync()`
  - **Note**: Moved to `/api/move` (validation is always performed)

- `POST /api/quantity/move/with-fifo` - Move with FIFO check
  - Takes: `MoveQuantityRequest`
  - Returns: `MoveQuantityResponse`
  - Service: `IQuantityMoveService.MoveQuantityWithFifoCheckAsync()`
  - **Note**: FIFO functionality removed entirely

---

## Removed Service Registrations

The following services are no longer registered in `Program.cs` but their implementations remain in the codebase:

### Services Removed from DI Container:
1. **IFifoService** / **FifoService**
   - Location: `backend/Services/Fifo/`
   - Purpose: FIFO (First-In-First-Out) validation and queries
   - Methods:
     - `GetOldestLotAsync()`
     - `ValidateFifoComplianceAsync()`
     - `GetFifoSummaryAsync()`

2. **IBarcodeService** / **BarcodeService**
   - Location: `backend/Services/Barcode/`
   - Purpose: Barcode parsing and lookup
   - Methods:
     - `ParseBarcodeAsync()`
     - `LookupBarcodeAsync()`
   - **Note**: Barcode parsing is now simple string split in the stock endpoint

3. **IStockValidationService** / **StockValidationService**
   - Location: `backend/Services/Stock/StockValidationService.cs`
   - Purpose: Stock-related validation operations
   - Methods:
     - `ValidateItemAsync()`
     - `ValidateLotAsync()`
     - `ValidateLocationAsync()`
     - `ValidateStockAvailabilityAsync()`
     - `ValidateStockForMoveAsync()`
   - **Note**: Some validation logic may still be used internally by other services

4. **IStockLocationService** / **StockLocationService**
   - Location: `backend/Services/Stock/StockLocationService.cs`
   - Purpose: Location-specific stock queries
   - Methods:
     - `GetLocationsWithStockAsync()`
     - `GetLocationsWithoutStockAsync()`
     - `GetLocationDetailsAsync()`

5. **IStockOperationService** / **StockOperationService**
   - Location: `backend/Services/Composition/StockOperationService.cs`
   - Purpose: Composition service that combines multiple smaller services
   - **Note**: Complex operations service, no longer needed with simplified API

---

## Services Still Registered (But Limited Usage)

These services remain registered but are only used by the 4 remaining endpoints:

1. **IStockQueryService** / **StockQueryService**
   - Used by: `GET /api/stock/{barcode}`
   - Method: `GetLocationsForItemLotAsync()`
   - Other methods in service are unused: `GetQuantityAtLocationAsync()`, `GetStockSummaryAsync()`, `GetCurrentLocationAsync()`

2. **IQuantityMoveService** / **QuantityMoveService**
   - Used by: `POST /api/move`
   - Method: `MoveQuantityAsync()` (which calls the `move_quantity` stored procedure)
   - Other methods unused: `MoveQuantityWithValidationAsync()`, `MoveQuantityWithFifoCheckAsync()`

3. **IQuantityValidationService** / **QuantityValidationService**
   - Used by: `POST /api/move/validate` and `POST /api/move`
   - Method: `ValidateMoveAsync()`
   - Other methods unused: `ValidateSourceStockAsync()`, `ValidateTargetLocationAsync()`

---

## Models That May Be Unused

The following model classes may no longer be used (but remain in codebase):

### Request Models:
- `BarcodeParseResponse`
- `BarcodeLookupResponse`
- `FifoValidationRequest`
- `FifoValidationResponse`
- `FifoOldestLotResponse`
- `FifoSummaryResponse`
- `GetQuantityRequest`
- `GetLocationsRequest`
- `GetCurrentLocationRequest`
- `StockMoveValidationRequest`
- `ValidateItemRequest`
- `ValidateLotRequest`
- `ValidateLocationRequest`
- `ValidateStockRequest`
- `ValidateSourceRequest`
- `ValidateTargetRequest`
- `MoveQuantityOptions`

### Response Models:
- `ItemValidationResponse`
- `LotValidationResponse`
- `LocationValidationResponse`
- `StockAvailabilityResponse`
- `CombinedValidationResponse`
- `LocationsWithStockResponse`
- `CurrentLocationResponse`
- `StockSummaryResponse`
- `ValidationResponse` (may still be used internally)

---

## How to Reimplement Removed Features

### To Re-add FIFO Functionality:
1. Re-register `IFifoService` in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IFifoService, FifoService>();
   ```
2. Re-add `FifoController.cs` or add endpoints to existing controller
3. Update frontend to call FIFO endpoints

### To Re-add Barcode Service:
1. Re-register `IBarcodeService` in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IBarcodeService, BarcodeService>();
   ```
2. Re-add `BarcodeController.cs` or integrate into existing endpoints
3. The service implementations should still exist in `backend/Services/Barcode/`

### To Re-add Stock Validation Endpoints:
1. Re-register `IStockValidationService` in `Program.cs`:
   ```csharp
   builder.Services.AddScoped<IStockValidationService, StockValidationService>();
   ```
2. Add endpoints back to `StockController` or create new validation controller
3. Service implementation should still exist in `backend/Services/Stock/`

### To Re-add Location Validation:
1. `IStockValidationService` includes location validation methods
2. Re-add the validation endpoints as described above
3. Or use the validation service methods internally in other endpoints

### To Re-add Separate Quantity Endpoints:
1. Re-add endpoints to `QuantityController` or create new controller
2. Methods are available in `IQuantityMoveService` and `IQuantityValidationService`
3. These services are still registered, just not exposed via endpoints

---

## Notes

- **Service implementations remain**: Most service classes still exist in the codebase, they're just not registered in DI or exposed via endpoints
- **Database stored procedures**: No stored procedures were removed, all still exist in the database
- **Models preserved**: All model classes remain in the codebase for potential future use
- **Tests**: Unit tests for removed controllers may need to be updated or removed
- **Frontend**: Frontend code using removed endpoints will need to be updated to use the new simplified endpoints

