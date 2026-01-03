namespace quantity_move_api.Common;

/// <summary>
/// Helper class for structured business event logging
/// Uses consistent format for easy searching and parsing
/// </summary>
public static class BusinessEventLogger
{
    /// <summary>
    /// Logs a move operation started event
    /// </summary>
    public static void LogMoveOperationStarted(ILogger logger, string itemCode, string sourceLocation, string targetLocation, decimal quantity, string? warehouseCode = null, string? lotNumber = null, string? correlationId = null)
    {
        logger.LogInformation(
            "BusinessEvent: MoveOperationStarted | Item: {ItemCode} | Source: {SourceLocation} | Target: {TargetLocation} | Quantity: {Quantity} | Warehouse: {WarehouseCode} | Lot: {LotNumber} | CorrelationId: {CorrelationId}",
            itemCode, sourceLocation, targetLocation, quantity, warehouseCode ?? "N/A", lotNumber ?? "N/A", correlationId ?? "N/A");
    }

    /// <summary>
    /// Logs a move operation completed event
    /// </summary>
    public static void LogMoveOperationCompleted(ILogger logger, bool success, long? transactionId, int returnCode, string? errorMessage = null, string? correlationId = null)
    {
        logger.LogInformation(
            "BusinessEvent: MoveOperationCompleted | Success: {Success} | TransactionId: {TransactionId} | ReturnCode: {ReturnCode} | ErrorMessage: {ErrorMessage} | CorrelationId: {CorrelationId}",
            success, transactionId ?? 0, returnCode, errorMessage ?? "N/A", correlationId ?? "N/A");
    }

    /// <summary>
    /// Logs a validation failed event
    /// </summary>
    public static void LogValidationFailed(ILogger logger, string validationType, string reason, string? itemCode = null, string? correlationId = null)
    {
        logger.LogWarning(
            "BusinessEvent: ValidationFailed | Type: {ValidationType} | Reason: {Reason} | Item: {ItemCode} | CorrelationId: {CorrelationId}",
            validationType, reason, itemCode ?? "N/A", correlationId ?? "N/A");
    }

    /// <summary>
    /// Logs a validation passed event
    /// </summary>
    public static void LogValidationPassed(ILogger logger, string validationType, string? itemCode = null, string? correlationId = null)
    {
        logger.LogInformation(
            "BusinessEvent: ValidationPassed | Type: {ValidationType} | Item: {ItemCode} | CorrelationId: {CorrelationId}",
            validationType, itemCode ?? "N/A", correlationId ?? "N/A");
    }

    /// <summary>
    /// Logs a FIFO violation detected event
    /// </summary>
    public static void LogFifoViolation(ILogger logger, string itemCode, string lotNumber, string warningMessage, string? correlationId = null)
    {
        logger.LogWarning(
            "BusinessEvent: FifoViolation | Item: {ItemCode} | Lot: {LotNumber} | Warning: {WarningMessage} | CorrelationId: {CorrelationId}",
            itemCode, lotNumber, warningMessage, correlationId ?? "N/A");
    }

    /// <summary>
    /// Logs a database operation event
    /// </summary>
    public static void LogDatabaseOperation(ILogger logger, string operation, string procedureName, long durationMs, bool success, string? correlationId = null)
    {
        var level = success ? LogLevel.Debug : LogLevel.Warning;
        logger.Log(level,
            "BusinessEvent: DatabaseOperation | Operation: {Operation} | Procedure: {ProcedureName} | Duration: {DurationMs}ms | Success: {Success} | CorrelationId: {CorrelationId}",
            operation, procedureName, durationMs, success, correlationId ?? "N/A");
    }
}

