# Quantity Move API

A modern, modular REST API for warehouse quantity movement operations with lot tracking, FIFO compliance, and barcode support.

## Features

- ✅ **Modular Architecture**: Focused services with single responsibilities
- ✅ **Lot Tracking**: Full support for lot-tracked inventory
- ✅ **FIFO Compliance**: First-In-First-Out validation
- ✅ **Barcode Support**: Parse and lookup barcodes (item%lot%quantity format)
- ✅ **Comprehensive Validation**: Multi-level validation before operations
- ✅ **JWT Authentication**: Secure API access
- ✅ **Snake Case JSON**: Consistent API naming convention
- ✅ **Default Values**: Automatic warehouse and site defaults
- ✅ **Refactored Codebase**: Clean architecture with BaseService, IQueryService, and constants

## Architecture

The API follows a **modular architecture** with clear separation of concerns:

```
Controllers → Services → Query Service → Database Service → SQL Server
```

### Key Components

- **BaseService<T>**: Abstract base class providing common functionality (logging, configuration, defaults)
- **IQueryService**: Abstraction layer for database queries (eliminates direct SqlConnection usage)
- **Constants**: Table names, column names, and stored procedure names centralized
- **ConfigurationService**: Centralized configuration access with default values
- **Custom Exceptions**: Domain-specific exceptions for better error handling

### Service Organization

- **Stock Services**: Query, validation, and location operations
- **Quantity Services**: Movement operations and pre-move validation
- **FIFO Service**: FIFO compliance checking
- **Barcode Service**: Barcode parsing and lookup
- **Composition Service**: Combines services for complex workflows

See [API_DOCUMENTATION.md](API_DOCUMENTATION.md) for complete architecture diagrams and workflows.

## Quick Start

### Prerequisites

- .NET 8.0 SDK
- SQL Server (with required tables and stored procedures)
- JWT secret key configured

### Configuration

Update `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Your connection string"
  },
  "Jwt": {
    "SecretKey": "Your secret key (min 32 characters)",
    "Issuer": "quantity-move-api",
    "Audience": "quantity-move-client",
    "ExpirationInHours": 24
  },
  "Database": {
    "Tables": {
      "LotLocation": "lot_loc_mst",
      "ItemMaster": "item_mst",
      "LocationMaster": "location_mst",
      "LotMaster": "lot_mst",
      "UserMaster": "user_master",
      "FifoSummary": "fifo_summary"
    }
  },
  "StoredProcedures": {
    "ValidateStock": "validate_stock",
    "MoveQuantity": "move_quantity"
  },
  "Defaults": {
    "DefaultWarehouse": "MAIN",
    "DefaultSite": "Default",
    "QualityControlEnabled": false
  }
}
```

**Note**: If `warehouse_code` or `site_reference` are not provided in API requests, the values from `Defaults` section will be used automatically.

### Running the API

```bash
cd backend
dotnet run
```

The API will be available at `https://localhost:5001/api` (or configured port).

### Using the API

📖 **New to the API?** Start with the **[GETTING_STARTED.md](GETTING_STARTED.md)** guide which includes:
- Step-by-step business logic scenarios
- What each endpoint does and what it looks up
- Complete request/response examples
- Common workflows and best practices

## API Endpoints

### Stock Operations (`/api/stock`)

**Query Endpoints**:
- `GET /api/stock/quantity` - Get quantity at location
- `GET /api/stock/locations` - Get locations for item/lot
- `GET /api/stock/locations/with-stock` - Get locations with stock
- `GET /api/stock/current-location` - Get current location(s)
- `GET /api/stock/summary` - Get stock summary

**Validation Endpoints**:
- `POST /api/stock/validate/item` - Validate item
- `POST /api/stock/validate/lot` - Validate lot
- `POST /api/stock/validate/location` - Validate location
- `POST /api/stock/validate/availability` - Validate stock availability
- `POST /api/stock/validate/move` - Combined validation for move

### Quantity Operations (`/api/quantity`)

**Validation Endpoints**:
- `POST /api/quantity/validate/source` - Validate source stock
- `POST /api/quantity/validate/target` - Validate target location
- `POST /api/quantity/validate/move` - Validate move operation

**Move Endpoints**:
- `POST /api/quantity/move` - Simple move
- `POST /api/quantity/move/with-validation` - Move with validation
- `POST /api/quantity/move/with-fifo` - Move with FIFO check

### FIFO Operations (`/api/fifo`)

- `GET /api/fifo/oldest-lot` - Get oldest lot for item
- `POST /api/fifo/validate` - Validate FIFO compliance
- `GET /api/fifo/summary` - Get FIFO summary

### Barcode Operations (`/api/barcode`)

- `POST /api/barcode/parse` - Parse barcode string
- `POST /api/barcode/lookup` - Lookup item from barcode

## Example Usage

### Get Quantity at Location

```bash
GET /api/stock/quantity?item_code=ABC123&lot_number=LOT001&location_code=LOC001&warehouse_code=WHSE01
```

### Move Quantity

```bash
POST /api/quantity/move
Content-Type: application/json

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

### Validate Stock for Move

```bash
POST /api/stock/validate/move
Content-Type: application/json

{
  "item_code": "ABC123",
  "lot_number": "LOT001",
  "source_location": "LOC001",
  "target_location": "LOC002",
  "quantity": 100.50,
  "warehouse_code": "WHSE01"
}
```

## Documentation

- **[GETTING_STARTED.md](GETTING_STARTED.md)**: **Start here!** Complete getting started guide with:
  - Business logic scenarios and which endpoints to use
  - What each endpoint looks up in the database
  - Request/response examples for common operations
  - Step-by-step workflows
  - Error handling guide

- **[API_DOCUMENTATION.md](API_DOCUMENTATION.md)**: Complete API documentation with:
  - Full methods list
  - Architecture diagrams
  - Workflow diagrams
  - Service explanations
  - Request/response examples

- **[IIS_DEPLOYMENT.md](IIS_DEPLOYMENT.md)**: IIS deployment guide

## Design Principles

1. **Single Responsibility**: Each method does one thing well
2. **Small Parameter Lists**: Max 3-4 parameters per method
3. **Composition Over Configuration**: Combine small methods rather than large parameterized ones
4. **Focused Services**: Separate services by domain concern
5. **Query Objects**: Use request DTOs instead of long parameter lists
6. **DRY (Don't Repeat Yourself)**: BaseService eliminates code duplication
7. **Abstraction**: IQueryService provides consistent database access
8. **Constants Over Magic Strings**: All table/column names in constants
9. **Configuration Centralization**: ConfigurationService for all config access
10. **Async Best Practices**: ConfigureAwait(false) throughout service layer

## Project Structure

```
backend/
├── Controllers/              # API controllers
│   ├── StockController.cs
│   ├── QuantityController.cs
│   ├── FifoController.cs
│   ├── BarcodeController.cs
│   └── AuthController.cs
├── Services/                 # Service layer
│   ├── Base/                 # BaseService abstract class
│   ├── Query/                # IQueryService and QueryService
│   ├── Stock/                # Stock-related services
│   ├── Quantity/             # Quantity movement services
│   ├── Fifo/                 # FIFO services
│   ├── Barcode/              # Barcode services
│   ├── Composition/         # Composition services
│   ├── DatabaseService.cs    # Database access layer
│   └── ConfigurationService.cs  # Configuration management
├── Common/
│   ├── Constants/            # TableNames, ColumnNames, StoredProcedureNames
│   ├── Exceptions/           # Custom exception classes
│   └── Builders/             # Response builders
├── Models/                   # Request/response models
├── Middleware/               # Exception handling middleware
└── Program.cs                # Application entry point
```

## Database Schema

### Core Tables

- `lot_loc_mst` - Lot location inventory
- `item_mst` - Item master data
- `location_mst` - Location master
- `lot_mst` - Lot master
- `fifo_summary` - FIFO summary

### Stored Procedures

- `validate_stock` - Stock validation
- `move_quantity` - Quantity movement

## Authentication

All endpoints require JWT authentication. Obtain a token via:

```bash
POST /api/auth/login
Content-Type: application/json

{
  "username": "your_username",
  "password": "your_password"
}
```

Include the token in subsequent requests:

```
Authorization: Bearer <your_token>
```

## Code Quality

The codebase has been refactored to follow best practices:

- ✅ **No Code Duplication**: Connection string retrieval and validation centralized
- ✅ **Consistent Database Access**: All services use IQueryService abstraction
- ✅ **Type Safety**: Magic strings replaced with constants
- ✅ **Structured Logging**: Consistent logging patterns throughout
- ✅ **Error Handling**: Custom exceptions and centralized error handling
- ✅ **Testability**: Services are easily mockable with dependency injection

## Testing

Run tests:

```bash
cd backend
dotnet test
```

## Development

### Building

```bash
cd backend
dotnet build
```

### Running in Development

```bash
cd backend
dotnet run
```

Swagger UI will be available at `https://localhost:5001/swagger` (in development mode).

## License

[Your License Here]

## Contributing

[Contributing Guidelines Here]

