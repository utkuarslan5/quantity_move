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
      "UserMaster": "TRM_EDIUSER",
      "FifoSummary": "TRM_FIFO_SUM"
    }
  },
  "StoredProcedures": {
    "ValidateStock": "TR_Stok_Kontrol",
    "MoveQuantity": "TR_Miktar_Ilerlet"
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

- `GET /api/stock/{barcode}` - Get stock locations and quantities by barcode
  - Barcode format: `item_code%lot_num` (e.g., `"ITEM123%LOT456"`)
  - Returns locations with stock for the item/lot combination

### Move Operations (`/api/move`)

- `POST /api/move/validate` - Validate if a move operation is allowed
  - Checks that source location has sufficient quantity and target location is valid
- `POST /api/move` - Execute a move operation
  - First validates the move, then executes it by calling the `TR_Miktar_Ilerlet` stored procedure
  - Includes automatic validation before execution

### Authentication (`/api/auth`)

- `POST /api/auth/login` - Authenticate user and get JWT token
  - Public endpoint (no authentication required)
  - Returns JWT token for subsequent API calls

### Health Check (`/api/health`)

- `GET /api/health` - Basic health check (service is running)
- `GET /api/health/ready` - Readiness check (service is ready to accept requests)
  - All health endpoints are public (no authentication required)

## Example Usage

### Get Stock by Barcode

```bash
GET /api/stock/ITEM123%LOT456
Authorization: Bearer <your_token>
```

**Response**:
```json
{
  "success": true,
  "data": {
    "item_code": "ITEM123",
    "lot_number": "LOT456",
    "locations": [
      {
        "location_code": "LOC001",
        "quantity": 100.50
      },
      {
        "location_code": "LOC002",
        "quantity": 50.25
      }
    ]
  }
}
```

### Move Quantity

```bash
POST /api/move
Authorization: Bearer <your_token>
Content-Type: application/json

{
  "item_code": "ABC123",
  "source_location": "LOC001",
  "source_lot_number": "LOT001",
  "target_location": "LOC002",
  "quantity": 100.50,
  "warehouse_code": "WHSE01",
  "site_reference": "SITE01",
  "document_number": "DOC123"
}
```

**Response**:
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

### Validate Move

```bash
POST /api/move/validate
Authorization: Bearer <your_token>
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

**Response**:
```json
{
  "success": true,
  "data": {
    "is_valid": true,
    "error_message": null
  }
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

- **[DATABASE_INTEGRATION.md](DATABASE_INTEGRATION.md)**: Complete database integration documentation with:
  - Table structures and column definitions
  - Stored procedure parameters and usage
  - Legacy database compatibility notes
  - Verification checklist for production connection

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
│   ├── MoveController.cs
│   ├── AuthController.cs
│   ├── HealthController.cs
│   └── BaseController.cs
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

The API is designed to work with the legacy database schema. See **[DATABASE_INTEGRATION.md](DATABASE_INTEGRATION.md)** for complete database documentation.

### Core Tables

- `lot_loc_mst` - Lot location inventory
- `item_mst` - Item master data
- `location_mst` - Location master
- `lot_mst` - Lot master
- `TRM_FIFO_SUM` - FIFO summary (legacy table name)
- `TRM_EDIUSER` - User authentication (legacy table name)
- `employee_mst` - Employee master

### Stored Procedures

- `TR_Stok_Kontrol` - Stock validation (legacy stored procedure)
- `TR_Miktar_Ilerlet` - Quantity movement (legacy stored procedure)

**Note**: The API uses legacy database table and stored procedure names for compatibility with the existing production database.

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

