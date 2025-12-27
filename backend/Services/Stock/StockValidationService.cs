using Dapper;
using quantity_move_api.Common.Builders;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;
using System.Data;

namespace quantity_move_api.Services.Stock;

public class StockValidationService : BaseService<StockValidationService>, IStockValidationService
{
    private readonly IQueryService _queryService;

    public StockValidationService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<StockValidationService> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public async Task<ItemValidationResponse> ValidateItemAsync(string itemCode, string? siteReference = null)
    {
        Logger.LogInformation("Validating item {ItemCode}, site {SiteReference}", itemCode, siteReference ?? "default");

        var query = $@"
            SELECT 
                {ColumnNames.ItemCode} AS ItemCode,
                {ColumnNames.LotTracked} AS IsLotTracked,
                {ColumnNames.Description}
            FROM {TableNames.ItemMaster} 
            WHERE {ColumnNames.ItemCode} = @ItemCode";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);

        if (!string.IsNullOrEmpty(siteReference))
        {
            query += $" AND {ColumnNames.SiteReference} = @SiteReference";
            parameters.Add("SiteReference", siteReference);
        }

        var result = await _queryService.QueryFirstOrDefaultAsync<ItemValidationResponse>(query, parameters).ConfigureAwait(false);

        if (result == null)
        {
            Logger.LogWarning("Item {ItemCode} not found", itemCode);
            return ValidationResponseBuilder.ItemNotFound(itemCode);
        }

        result.IsValid = true;
        if (!result.IsLotTracked)
        {
            result.ErrorMessage = "Item is not lot-tracked";
        }

        return result;
    }

    public async Task<LotValidationResponse> ValidateLotAsync(string itemCode, string lotNumber)
    {
        Logger.LogInformation("Validating lot {LotNumber} for item {ItemCode}", lotNumber, itemCode);

        var query = $@"
            SELECT COUNT(*) 
            FROM {TableNames.LotMaster} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.LotNumber} = @LotNumber";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("LotNumber", lotNumber);

        var count = await _queryService.QueryFirstOrDefaultAsync<int>(query, parameters).ConfigureAwait(false);

        if (count == 0)
        {
            Logger.LogWarning("Lot {LotNumber} not found for item {ItemCode}", lotNumber, itemCode);
            return ValidationResponseBuilder.LotNotFound(itemCode, lotNumber);
        }

        return ValidationResponseBuilder.LotSuccess(itemCode, lotNumber);
    }

    public async Task<LocationValidationResponse> ValidateLocationAsync(string locationCode, string? siteReference = null)
    {
        Logger.LogInformation("Validating location {LocationCode}, site {SiteReference}", locationCode, siteReference ?? "default");

        var query = $@"
            SELECT 
                {ColumnNames.LocationCode} AS LocationCode,
                {ColumnNames.LocationType} AS LocationType,
                {ColumnNames.Description}
            FROM {TableNames.LocationMaster} 
            WHERE {ColumnNames.LocationCode} = @LocationCode";

        var parameters = new DynamicParameters();
        parameters.Add("LocationCode", locationCode);

        if (!string.IsNullOrEmpty(siteReference))
        {
            query += $" AND {ColumnNames.SiteReference} = @SiteReference";
            parameters.Add("SiteReference", siteReference);
        }

        var result = await _queryService.QueryFirstOrDefaultAsync<LocationValidationResponse>(query, parameters).ConfigureAwait(false);

        if (result == null)
        {
            Logger.LogWarning("Location {LocationCode} not found", locationCode);
            return ValidationResponseBuilder.LocationNotFound(locationCode);
        }

        result.IsValid = true;
        return result;
    }

    public async Task<StockAvailabilityResponse> ValidateStockAvailabilityAsync(string itemCode, string lotNumber, string locationCode, decimal requiredQuantity, string warehouseCode)
    {
        Logger.LogInformation("Validating stock availability for item {ItemCode}, lot {LotNumber}, location {LocationCode}, warehouse {WarehouseCode}, required: {RequiredQuantity}", 
            itemCode, lotNumber, locationCode, warehouseCode, requiredQuantity);

        var query = $@"
            SELECT CAST({ColumnNames.QuantityOnHand} AS DECIMAL(18, 4)) AS AvailableQuantity
            FROM {TableNames.LotLocation} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.LotNumber} = @LotNumber 
              AND {ColumnNames.LocationCode} = @LocationCode 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("LotNumber", lotNumber);
        parameters.Add("LocationCode", locationCode);
        parameters.Add("WarehouseCode", warehouseCode);

        var availableQuantity = await _queryService.QueryFirstOrDefaultAsync<decimal?>(query, parameters).ConfigureAwait(false) ?? 0;

        var isAvailable = availableQuantity >= requiredQuantity;
        if (!isAvailable)
        {
            Logger.LogWarning("Insufficient stock for item {ItemCode} at location {LocationCode}. Available: {Available}, Required: {Required}", 
                itemCode, locationCode, availableQuantity, requiredQuantity);
            return ValidationResponseBuilder.InsufficientStock(itemCode, lotNumber, locationCode, availableQuantity, requiredQuantity);
        }

        return ValidationResponseBuilder.StockAvailable(itemCode, lotNumber, locationCode, availableQuantity, requiredQuantity);
    }

    public async Task<CombinedValidationResponse> ValidateStockForMoveAsync(StockMoveValidationRequest request)
    {
        var response = new CombinedValidationResponse();

        // Validate item
        var itemValidation = await ValidateItemAsync(request.ItemCode, request.SiteReference).ConfigureAwait(false);
        response.ItemValidation = itemValidation;
        if (!itemValidation.IsValid)
        {
            response.IsValid = false;
            response.ErrorMessage = itemValidation.ErrorMessage;
            return response;
        }

        // Validate lot
        var lotValidation = await ValidateLotAsync(request.ItemCode, request.LotNumber).ConfigureAwait(false);
        response.LotValidation = lotValidation;
        if (!lotValidation.IsValid)
        {
            response.IsValid = false;
            response.ErrorMessage = lotValidation.ErrorMessage;
            return response;
        }

        // Validate source location
        var sourceLocationValidation = await ValidateLocationAsync(request.SourceLocation, request.SiteReference).ConfigureAwait(false);
        response.SourceLocationValidation = sourceLocationValidation;
        if (!sourceLocationValidation.IsValid)
        {
            response.IsValid = false;
            response.ErrorMessage = $"Source location invalid: {sourceLocationValidation.ErrorMessage}";
            return response;
        }

        // Validate target location
        var targetLocationValidation = await ValidateLocationAsync(request.TargetLocation, request.SiteReference).ConfigureAwait(false);
        response.TargetLocationValidation = targetLocationValidation;
        if (!targetLocationValidation.IsValid)
        {
            response.IsValid = false;
            response.ErrorMessage = $"Target location invalid: {targetLocationValidation.ErrorMessage}";
            return response;
        }

        // Validate stock availability
        var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
        var stockAvailability = await ValidateStockAvailabilityAsync(
            request.ItemCode, request.LotNumber, request.SourceLocation, 
            request.Quantity, warehouseCode).ConfigureAwait(false);
        response.StockAvailability = stockAvailability;
        if (!stockAvailability.IsAvailable)
        {
            response.IsValid = false;
            response.ErrorMessage = stockAvailability.ErrorMessage;
            return response;
        }

        response.IsValid = true;
        return response;
    }
}
