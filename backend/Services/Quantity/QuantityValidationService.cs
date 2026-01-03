using Dapper;
using quantity_move_api.Common;
using quantity_move_api.Common.Constants;
using quantity_move_api.Common.Exceptions;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;

namespace quantity_move_api.Services.Quantity;

public class QuantityValidationService : BaseService<QuantityValidationService>, IQuantityValidationService
{
    private readonly IQueryService _queryService;

    public QuantityValidationService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<QuantityValidationService> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public async Task<ValidationResponse> ValidateSourceStockAsync(string itemCode, string lotNumber, string sourceLocation, decimal quantity, string warehouseCode)
    {
        Logger.LogInformation("Validating source stock for item {ItemCode}, lot {LotNumber}, location {SourceLocation}, warehouse {WarehouseCode}, quantity {Quantity}", 
            itemCode, lotNumber, sourceLocation, warehouseCode, quantity);

        var query = $@"
            SELECT CAST({ColumnNames.QuantityOnHand} AS DECIMAL(18, 4)) AS AvailableQuantity
            FROM {TableNames.LotLocation} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.LotNumber} = @LotNumber 
              AND {ColumnNames.LocationCode} = @SourceLocation 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("LotNumber", lotNumber);
        parameters.Add("SourceLocation", sourceLocation);
        parameters.Add("WarehouseCode", warehouseCode);

        var availableQuantity = await _queryService.QueryFirstOrDefaultAsync<decimal?>(query, parameters).ConfigureAwait(false) ?? 0;

        if (availableQuantity < quantity)
        {
            var errorMessage = $"Insufficient stock at source location. Available: {availableQuantity}, Required: {quantity}";
            Logger.LogWarning("Insufficient stock at source location {SourceLocation}. Available: {Available}, Required: {Required}", 
                sourceLocation, availableQuantity, quantity);
            BusinessEventLogger.LogValidationFailed(
                Logger,
                "SourceStock",
                errorMessage,
                itemCode,
                System.Diagnostics.Activity.Current?.Id);
            return new ValidationResponse
            {
                IsValid = false,
                ErrorMessage = errorMessage
            };
        }

        BusinessEventLogger.LogValidationPassed(
            Logger,
            "SourceStock",
            itemCode,
            System.Diagnostics.Activity.Current?.Id);
        return new ValidationResponse
        {
            IsValid = true
        };
    }

    public async Task<ValidationResponse> ValidateTargetLocationAsync(string itemCode, string targetLocation, string warehouseCode)
    {
        Logger.LogInformation("Validating target location {TargetLocation} for item {ItemCode}", targetLocation, itemCode);

        var query = $@"
            SELECT COUNT(*) 
            FROM {TableNames.LocationMaster} 
            WHERE {ColumnNames.LocationCode} = @TargetLocation";

        var parameters = new DynamicParameters();
        parameters.Add("TargetLocation", targetLocation);

        var count = await _queryService.QueryFirstOrDefaultAsync<int>(query, parameters).ConfigureAwait(false);

        if (count == 0)
        {
            var errorMessage = $"Target location {targetLocation} not found";
            Logger.LogWarning("Target location {TargetLocation} not found", targetLocation);
            BusinessEventLogger.LogValidationFailed(
                Logger,
                "TargetLocation",
                errorMessage,
                itemCode,
                System.Diagnostics.Activity.Current?.Id);
            return new ValidationResponse
            {
                IsValid = false,
                ErrorMessage = errorMessage
            };
        }

        BusinessEventLogger.LogValidationPassed(
            Logger,
            "TargetLocation",
            itemCode,
            System.Diagnostics.Activity.Current?.Id);
        return new ValidationResponse
        {
            IsValid = true
        };
    }

    public async Task<MoveValidationResponse> ValidateMoveAsync(MoveValidationRequest request)
    {
        var response = new MoveValidationResponse();
        var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);

        // Validate source stock
        var sourceValidation = await ValidateSourceStockAsync(
            request.ItemCode, request.LotNumber, request.SourceLocation, 
            request.Quantity, warehouseCode).ConfigureAwait(false);
        response.SourceValidation = sourceValidation;

        if (!sourceValidation.IsValid)
        {
            response.IsValid = false;
            response.ErrorMessage = sourceValidation.ErrorMessage;
            return response;
        }

        // Validate target location
        var targetValidation = await ValidateTargetLocationAsync(
            request.ItemCode, request.TargetLocation, warehouseCode).ConfigureAwait(false);
        response.TargetValidation = targetValidation;

        if (!targetValidation.IsValid)
        {
            response.IsValid = false;
            response.ErrorMessage = targetValidation.ErrorMessage;
            return response;
        }

        response.IsValid = true;
        return response;
    }
}

