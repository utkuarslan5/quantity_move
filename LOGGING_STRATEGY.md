# Logging Strategy for Quantity Move API

## Executive Summary

This document outlines a comprehensive logging strategy for the Quantity Move API, designed to provide observability, debugging capabilities, and audit trails for warehouse quantity movement operations.

## Current State Analysis

### Existing Logging Infrastructure
- ✅ **ILogger<T>** is used throughout the codebase
- ✅ **Correlation IDs** are implemented in ExceptionHandlingMiddleware
- ✅ Basic logging in controllers, services, and repositories
- ✅ Error logging in DatabaseService for database operations
- ⚠️ **No structured logging provider** (Serilog/NLog)
- ⚠️ **No file logging** configured
- ⚠️ **No request/response logging** middleware
- ⚠️ **Limited log levels** configuration
- ⚠️ **No log enrichment** (user context, request details)

### Current Logging Usage
- **Error Logging**: ExceptionHandlingMiddleware, DatabaseService, Controllers
- **Information Logging**: Services (AuthService, QuantityMoveService, StockQueryService)
- **Warning Logging**: Validation failures, insufficient stock, FIFO violations

## Logging Strategy

### 1. Structured Logging with Serilog

**Why Serilog?**
- Industry standard for .NET structured logging
- Rich formatting and output options
- Excellent performance
- Easy integration with ASP.NET Core
- Supports multiple sinks (file, console, database, etc.)

**Implementation:**
- Replace default logging with Serilog
- Configure structured JSON output for production
- Human-readable console output for development

### 2. Log Levels and Categories

#### Log Level Strategy
- **Trace**: Detailed diagnostic information (disabled in production)
- **Debug**: Development debugging information
- **Information**: General application flow, business operations
- **Warning**: Recoverable issues, validation failures, business rule violations
- **Error**: Exceptions, failures that need attention
- **Critical**: System failures, security breaches

#### Category-Specific Levels
```json
{
  "Logging": {
    "LogLevel": {
      "Default": "Information",
      "Microsoft.AspNetCore": "Warning",
      "Microsoft.AspNetCore.Hosting": "Information",
      "Microsoft.AspNetCore.Routing": "Warning",
      "Microsoft.EntityFrameworkCore": "Warning",
      "System.Net.Http.HttpClient": "Warning",
      "quantity_move_api": "Information",
      "quantity_move_api.Services": "Information",
      "quantity_move_api.Controllers": "Information",
      "quantity_move_api.Middleware": "Information",
      "quantity_move_api.Repositories": "Warning",
      "quantity_move_api.Services.DatabaseService": "Warning"
    }
  }
}
```

### 3. Logging Targets (Sinks)

#### Development Environment
- **Console**: Human-readable, colored output
- **File**: Rolling file logs (daily rotation)

#### Production Environment
- **File**: Rolling file logs (daily rotation, 30-day retention)
- **Console**: JSON structured output (for containerized deployments)
- **Future**: Consider Application Insights, Seq, or ELK stack

### 4. Log Enrichment

#### Standard Properties
- **CorrelationId**: Request correlation ID (already implemented)
- **RequestId**: Unique request identifier
- **UserId**: Authenticated user ID (when available)
- **Username**: Authenticated username (when available)
- **MachineName**: Server name
- **Environment**: Development/Staging/Production
- **Application**: "quantity-move-api"
- **Version**: Application version

#### Request Context
- HTTP Method
- Request Path
- Response Status Code
- Request Duration (milliseconds)
- Client IP Address
- User Agent

### 5. Logging Patterns by Component

#### Controllers
- **Entry**: Log incoming requests with key parameters (sanitized)
- **Exit**: Log response status and duration
- **Errors**: Log exceptions with full context

#### Services
- **Business Operations**: Log at Information level
  - Move operations: ItemCode, LotNumber, SourceLocation, TargetLocation, Quantity
  - Validation operations: Validation results
  - Query operations: Query parameters (sanitized)
- **Warnings**: Business rule violations, validation failures
- **Errors**: Exceptions with operation context

#### Repositories/Database
- **Queries**: Log at Warning level (only failures)
- **Stored Procedures**: Log execution time for slow queries (>1 second)
- **Errors**: Log all database errors with connection info (sanitized)

#### Middleware
- **Request Logging**: All HTTP requests (method, path, status, duration)
- **Exception Handling**: Enhanced error logging with stack traces
- **Authentication**: Log authentication attempts (success/failure)

### 6. Security and Privacy

#### Sensitive Data Handling
- **Never log**: Passwords, tokens, connection strings
- **Sanitize**: User input that might contain sensitive data
- **Mask**: Partial credit card numbers, SSNs (if applicable)
- **Redact**: Database connection strings (show only server name)

#### Audit Logging
- **Authentication Events**: Login attempts (success/failure), token generation
- **Authorization Events**: Access denied events
- **Data Modifications**: Critical operations (move operations, stock adjustments)
- **Configuration Changes**: Changes to system configuration

### 7. Performance Logging

#### Metrics to Log
- **Request Duration**: Track slow requests (>1 second)
- **Database Query Time**: Log slow queries (>500ms)
- **Stored Procedure Execution**: Log execution time
- **Memory Usage**: Periodic memory snapshots (Warning level)

### 8. File Logging Configuration

#### File Structure
```
logs/
  ├── app-{Date}.log          # General application logs
  ├── errors-{Date}.log       # Error-level logs only
  ├── audit-{Date}.log        # Audit trail logs
  └── performance-{Date}.log  # Performance metrics
```

#### Rotation Policy
- **Daily rotation**: New file each day
- **Retention**: 30 days (configurable)
- **Size limit**: 100MB per file (rollover if exceeded)
- **Compression**: Compress files older than 7 days

### 9. Request/Response Logging Middleware

#### What to Log
- **Request**: Method, Path, Query String (sanitized), Headers (selected), Body (sanitized, max 1KB)
- **Response**: Status Code, Headers (selected), Body (sanitized, max 1KB), Duration

#### What NOT to Log
- Authorization headers (tokens)
- Large request/response bodies (>1KB)
- Binary content
- Sensitive form data

### 10. Error Logging Best Practices

#### Exception Logging
- Always include exception object (not just message)
- Include correlation ID
- Include user context (if available)
- Include request details (path, method)
- Include business context (operation being performed)

#### Example Pattern
```csharp
_logger.LogError(ex, 
    "Error during {Operation}. CorrelationId: {CorrelationId}, User: {Username}, Item: {ItemCode}",
    operation, correlationId, username, itemCode);
```

### 11. Logging Configuration by Environment

#### Development
- Console: Human-readable, colored
- File: Detailed logs (Debug level for development)
- Include stack traces for all errors

#### Staging
- Console: JSON structured
- File: Information level
- Include performance metrics

#### Production
- Console: JSON structured (for containers)
- File: Information level, Error level separate
- Minimal stack traces (only for errors)
- Performance metrics enabled

## Implementation Plan

### Phase 1: Foundation (Immediate)
1. ✅ Install Serilog packages
2. ✅ Configure Serilog in Program.cs
3. ✅ Set up file logging with rotation
4. ✅ Configure log levels per category
5. ✅ Add log enrichment (correlation ID, request context)

### Phase 2: Enhanced Logging (Short-term)
1. ✅ Create RequestLoggingMiddleware
2. ✅ Enhance exception logging with context
3. ✅ Add performance logging
4. ✅ Implement audit logging for critical operations

### Phase 3: Advanced Features (Medium-term)
1. Add structured logging to all services
2. Implement log aggregation (Seq/Application Insights)
3. Add log analytics and dashboards
4. Set up alerting based on log patterns

## Logging Checklist

### For Each New Feature
- [ ] Log entry point with key parameters
- [ ] Log business operations at Information level
- [ ] Log validation failures at Warning level
- [ ] Log exceptions at Error level with full context
- [ ] Include correlation ID in all logs
- [ ] Sanitize sensitive data before logging

### Code Review Checklist
- [ ] No sensitive data in logs (passwords, tokens)
- [ ] Appropriate log level used
- [ ] Correlation ID included
- [ ] Exception object logged (not just message)
- [ ] Business context included in error logs

## Monitoring and Alerting

### Key Metrics to Monitor
- Error rate (errors per minute)
- Request duration (p95, p99)
- Database query performance
- Authentication failure rate
- Critical exceptions

### Alert Thresholds
- Error rate > 10 errors/minute
- Request duration p99 > 5 seconds
- Database connection failures
- Authentication failures > 5/minute (potential attack)

## Conclusion

This logging strategy provides:
- **Observability**: Full visibility into application behavior
- **Debugging**: Rich context for troubleshooting
- **Audit Trail**: Compliance and security tracking
- **Performance Monitoring**: Identify bottlenecks
- **Security**: Track authentication and authorization events

The implementation uses industry-standard tools (Serilog) and follows .NET best practices for structured logging.

