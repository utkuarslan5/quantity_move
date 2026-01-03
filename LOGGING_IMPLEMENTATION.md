# Logging Implementation Summary

## Overview

A comprehensive logging strategy has been implemented for the Quantity Move API using **Serilog** for structured logging. This provides observability, debugging capabilities, and audit trails for warehouse quantity movement operations.

## What Was Implemented

### 1. Serilog Integration ✅

**Packages Added:**
- `Serilog.AspNetCore` (8.0.3) - Main Serilog integration
- `Serilog.Sinks.Console` (6.0.0) - Console output
- `Serilog.Sinks.File` (6.0.0) - File logging
- `Serilog.Enrichers.Environment` (3.0.1) - Environment enrichment
- `Serilog.Enrichers.Thread` (4.0.0) - Thread information
- `Serilog.Settings.Configuration` (8.0.4) - Configuration support

**Configuration:**
- Configured in `Program.cs` with environment-specific settings
- Development: Human-readable console output
- Production: JSON structured output
- File logging with daily rotation and 30-day retention

### 2. Request Logging Middleware ✅

**New File:** `backend/Middleware/RequestLoggingMiddleware.cs`

**Features:**
- Logs all HTTP requests and responses
- Captures request/response bodies (sanitized, max 1KB)
- Tracks request duration
- Logs at appropriate levels:
  - Information: Normal requests
  - Warning: Slow requests (>1s) or 4xx errors
  - Error: 5xx server errors
- Includes user context (username, user ID)
- Sanitizes sensitive data (passwords, JWT tokens)

**Log Properties:**
- CorrelationId
- RequestId
- RequestPath
- RequestMethod
- Username
- UserId
- RemoteIpAddress
- Duration (milliseconds)

### 3. Enhanced Exception Handling ✅

**Updated:** `backend/Middleware/ExceptionHandlingMiddleware.cs`

**Improvements:**
- Enhanced error logging with Serilog LogContext
- Includes full request context in error logs
- Logs user information when available
- Includes IP address for security tracking

### 4. Log Configuration ✅

**Updated Files:**
- `appsettings.json` - Production log levels
- `appsettings.Development.json` - Development log levels (more verbose)

**Log Level Strategy:**
- **Default**: Information
- **Microsoft.AspNetCore**: Warning (reduces framework noise)
- **Services**: Information (business operations)
- **Repositories**: Warning (only failures)
- **DatabaseService**: Warning (only failures)
- **Middleware**: Information (request tracking)

### 5. File Logging ✅

**Log Files Created:**
- `logs/app-{Date}.log` - All application logs
- `logs/errors-{Date}.log` - Warning and Error level logs only

**Rotation Policy:**
- Daily rotation
- 30-day retention
- 100MB file size limit (rollover if exceeded)
- Shared file access for multiple processes

### 6. Log Enrichment ✅

**Standard Properties Added:**
- Application name: "quantity-move-api"
- Machine name
- Environment name
- Correlation ID (from middleware)
- Request ID
- User context (when authenticated)

## Logging Patterns

### Controllers
```csharp
_logger.LogInformation("Operation started. User: {Username}", username);
_logger.LogError(ex, "Error during {Operation}", operation);
```

### Services
```csharp
Logger.LogInformation("Validating user {Username}", username);
Logger.LogWarning("User {Username} not found", username);
Logger.LogError(ex, "Error validating user {Username}", username);
```

### Middleware
- RequestLoggingMiddleware: Logs all HTTP requests/responses
- ExceptionHandlingMiddleware: Logs unhandled exceptions with full context

## Security Considerations

### Sensitive Data Handling
- ✅ Passwords are never logged
- ✅ JWT tokens are redacted in logs
- ✅ Request/response bodies are limited to 1KB
- ✅ Database connection strings are sanitized

### Audit Logging
- Authentication events (login attempts)
- Authorization failures
- Critical operations (move operations)
- All errors with user context

## Usage Examples

### Viewing Logs

**Development:**
```bash
# Console output (colored, human-readable)
# File logs in logs/ directory
tail -f logs/app-2024-01-15.log
```

**Production:**
```bash
# JSON structured logs
tail -f logs/app-2024-01-15.log | jq

# Error logs only
tail -f logs/errors-2024-01-15.log
```

### Searching Logs

```bash
# Find all errors for a specific user
grep "Username.*john" logs/errors-*.log

# Find slow requests
grep "Duration.*[0-9][0-9][0-9][0-9]" logs/app-*.log

# Find requests by correlation ID
grep "CorrelationId.*abc-123" logs/app-*.log
```

## Configuration

### Environment Variables

You can override log levels using environment variables:
```bash
export Serilog__MinimumLevel__Default=Debug
export Serilog__MinimumLevel__Override__Microsoft=Warning
```

### appsettings.json

Log levels can be configured in `appsettings.json`:
```json
{
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "quantity_move_api": "Information"
      }
    }
  }
}
```

## Next Steps (Future Enhancements)

1. **Log Aggregation**: Integrate with Seq, Application Insights, or ELK stack
2. **Performance Metrics**: Add detailed performance logging for database queries
3. **Audit Logging**: Separate audit log file for compliance
4. **Alerting**: Set up alerts based on error rates and patterns
5. **Dashboards**: Create log analytics dashboards

## Testing

To test the logging implementation:

1. **Start the application**: Logs will be written to console and files
2. **Make API requests**: Check `logs/app-*.log` for request logs
3. **Trigger an error**: Check `logs/errors-*.log` for error details
4. **Check correlation IDs**: Verify correlation IDs are consistent across logs

## Troubleshooting

### Logs Not Appearing
- Check file permissions on `logs/` directory
- Verify Serilog packages are restored
- Check log level configuration

### Too Many Logs
- Adjust log levels in `appsettings.json`
- Increase minimum level for verbose namespaces

### Missing Context
- Ensure RequestLoggingMiddleware is registered early in pipeline
- Verify correlation ID is being set in ExceptionHandlingMiddleware

## Files Modified

1. ✅ `backend/quantity_move_api.csproj` - Added Serilog packages
2. ✅ `backend/Program.cs` - Configured Serilog
3. ✅ `backend/Middleware/RequestLoggingMiddleware.cs` - New file
4. ✅ `backend/Middleware/ExceptionHandlingMiddleware.cs` - Enhanced logging
5. ✅ `backend/appsettings.json` - Log level configuration
6. ✅ `backend/appsettings.Development.json` - Development log levels
7. ✅ `.gitignore` - Exclude log files

## Documentation

- **Strategy Document**: `LOGGING_STRATEGY.md` - Comprehensive logging strategy
- **Implementation Summary**: This document

## Conclusion

The logging implementation provides:
- ✅ **Observability**: Full visibility into application behavior
- ✅ **Debugging**: Rich context for troubleshooting
- ✅ **Audit Trail**: Compliance and security tracking
- ✅ **Performance Monitoring**: Request duration tracking
- ✅ **Security**: Authentication and authorization event tracking

The implementation follows industry best practices and is ready for production use.

