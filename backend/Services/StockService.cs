using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;
using System.Data;

namespace quantity_move_api.Services;

public class StockService : BaseService<StockService>, IStockService
{
    private readonly IQueryService _queryService;

    public StockService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<StockService> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public async Task<StockValidationResponse> ValidateStockAsync(StockValidationRequest request)
    {
        Logger.LogInformation("Validating stock for item {ItemCode}, lot {LotNumber}, location {LocationCode}", 
            request.ItemCode, request.LotNumber, request.LocationCode);

        var procedureName = ConfigurationService.GetStoredProcedureName("ValidateStock");
        
        var parameters = new DynamicParameters();
        parameters.Add("@item_code", request.ItemCode);
        parameters.Add("@lot_number", request.LotNumber);
        parameters.Add("@location_code", request.LocationCode);
        
        if (!string.IsNullOrEmpty(request.WarehouseCode))
            parameters.Add("@warehouse_code", request.WarehouseCode);
        if (!string.IsNullOrEmpty(request.SiteReference))
            parameters.Add("@site_reference", request.SiteReference);
        if (request.ValidationDate.HasValue)
            parameters.Add("@validation_date", request.ValidationDate);
        if (request.RequiredQuantity.HasValue)
            parameters.Add("@required_quantity", request.RequiredQuantity);
        
        parameters.Add("@check_quantity_available", request.CheckQuantityAvailable);
        parameters.Add("@return_location_info", request.ReturnLocationInfo);
        
        // Output parameters
        parameters.Add("@return_code", dbType: DbType.Int32, direction: ParameterDirection.Output);
        parameters.Add("@available_quantity", dbType: DbType.Decimal, direction: ParameterDirection.Output);
        parameters.Add("@error_message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);
        parameters.Add("@location_type", dbType: DbType.String, size: 50, direction: ParameterDirection.Output);
        parameters.Add("@location_description", dbType: DbType.String, size: 200, direction: ParameterDirection.Output);

        await DatabaseService.ExecuteStoredProcedureNonQueryAsync(procedureName, parameters).ConfigureAwait(false);

        var returnCode = parameters.Get<int>("@return_code");
        var availableQuantity = parameters.Get<decimal?>("@available_quantity");
        var errorMessage = parameters.Get<string>("@error_message");
        var locationType = parameters.Get<string>("@location_type");
        var locationDescription = parameters.Get<string>("@location_description");

        var response = new StockValidationResponse
        {
            IsValid = returnCode == 0,
            ReturnCode = returnCode,
            ErrorMessage = errorMessage,
            AvailableQuantity = availableQuantity
        };

        // Include location info if requested
        if (request.ReturnLocationInfo && !string.IsNullOrEmpty(locationType))
        {
            response.LocationInfo = new LocationInfo
            {
                LocationCode = request.LocationCode,
                LocationType = locationType ?? string.Empty,
                Description = locationDescription
            };
        }

        return response;
    }

    public async Task<QuantityOnHandResponse> GetQuantityOnHandAsync(QuantityOnHandRequest request)
    {
        Logger.LogInformation("Getting quantity on hand for item {ItemCode}, location {LocationCode}, lot {LotNumber}", 
            request.ItemCode, request.LocationCode, request.LotNumber);

        var query = $@"
            SELECT 
                {ColumnNames.ItemCode} AS ItemCode,
                {ColumnNames.LocationCode} AS LocationCode,
                {ColumnNames.LotNumber} AS LotNumber,
                CAST({ColumnNames.QuantityOnHand} AS DECIMAL(18, 2)) AS QuantityOnHand
            FROM {TableNames.LotLocation}
            WHERE {ColumnNames.ItemCode} = @ItemCode
              AND {ColumnNames.LocationCode} = @LocationCode
              AND {ColumnNames.QuantityOnHand} > 0";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", request.ItemCode);
        parameters.Add("LocationCode", request.LocationCode);

        if (!string.IsNullOrEmpty(request.LotNumber))
        {
            query += $" AND {ColumnNames.LotNumber} = @LotNumber";
            parameters.Add("LotNumber", request.LotNumber);
        }
        else
        {
            query += $" AND {ColumnNames.LotNumber} IS NULL";
        }

        if (!string.IsNullOrEmpty(request.WarehouseCode))
        {
            query += $" AND {ColumnNames.WarehouseCode} = @WarehouseCode";
            parameters.Add("WarehouseCode", request.WarehouseCode);
        }

        if (!string.IsNullOrEmpty(request.SiteReference))
        {
            query += $" AND {ColumnNames.SiteReference} = @SiteReference";
            parameters.Add("SiteReference", request.SiteReference);
        }

        var result = await _queryService.QueryFirstOrDefaultAsync<QuantityOnHandResponse>(query, parameters).ConfigureAwait(false);

        if (result == null)
        {
            return new QuantityOnHandResponse
            {
                ItemCode = request.ItemCode,
                LocationCode = request.LocationCode,
                LotNumber = request.LotNumber,
                QuantityOnHand = 0
            };
        }

        return result;
    }

    public async Task<StockInformationResponse> GetStockInformationAsync(StockInformationRequest request)
    {
        Logger.LogInformation("Getting stock information for item {ItemCode}, lot {LotNumber}, location {LocationCode}, warehouse {WarehouseCode}", 
            request.ItemCode, request.LotNumber, request.LocationCode, request.WarehouseCode);

        var query = $@"
            SELECT 
                lot_loc.{ColumnNames.ItemCode} AS ItemCode,
                lot_loc.{ColumnNames.LocationCode} AS LocationCode,
                lot_loc.{ColumnNames.LotNumber} AS LotNumber,
                CAST(lot_loc.{ColumnNames.QuantityOnHand} AS DECIMAL(18, 2)) AS QuantityOnHand,
                (SELECT ISNULL({ColumnNames.CreateDate}, {ColumnNames.RecordDate}) AS fifo_date 
                 FROM {TableNames.LotMaster} lot 
                 WHERE lot.{ColumnNames.ItemCode} = lot_loc.{ColumnNames.ItemCode} 
                   AND lot.{ColumnNames.LotNumber} = lot_loc.{ColumnNames.LotNumber}) AS FifoDate";

        if (request.IncludeFifoValidation)
        {
            query += $@",
                (SELECT TOP 1 {ColumnNames.LotNumber} 
                 FROM {TableNames.FifoSummary} 
                 WHERE {ColumnNames.ItemCode} = lot_loc.{ColumnNames.ItemCode} 
                   AND {ColumnNames.WarehouseCode} = lot_loc.{ColumnNames.WarehouseCode}
                   AND (@SiteReference IS NULL OR {ColumnNames.SiteReference} = @SiteReference)
                 ORDER BY {ColumnNames.FifoDate} ASC) AS OldestLot";
        }
        else
        {
            query += ", NULL AS OldestLot";
        }

        query += $@",
                lot_loc.{ColumnNames.WarehouseCode} AS WarehouseCode,
                lot_loc.{ColumnNames.SiteReference} AS SiteReference
            FROM {TableNames.LotLocation} lot_loc 
            WHERE lot_loc.{ColumnNames.QuantityOnHand} > 0";

        var parameters = new DynamicParameters();

        // Always add SiteReference parameter for subquery use, even if null
        parameters.Add("SiteReference", request.SiteReference ?? (object)DBNull.Value);

        if (!string.IsNullOrEmpty(request.ItemCode))
        {
            query += $" AND lot_loc.{ColumnNames.ItemCode} = @ItemCode";
            parameters.Add("ItemCode", request.ItemCode);
        }

        if (!string.IsNullOrEmpty(request.LotNumber))
        {
            query += $" AND lot_loc.{ColumnNames.LotNumber} = @LotNumber";
            parameters.Add("LotNumber", request.LotNumber);
        }

        if (!string.IsNullOrEmpty(request.LocationCode))
        {
            query += $" AND lot_loc.{ColumnNames.LocationCode} = @LocationCode";
            parameters.Add("LocationCode", request.LocationCode);
        }

        if (!string.IsNullOrEmpty(request.WarehouseCode))
        {
            query += $" AND lot_loc.{ColumnNames.WarehouseCode} = @WarehouseCode";
            parameters.Add("WarehouseCode", request.WarehouseCode);
        }

        if (!string.IsNullOrEmpty(request.SiteReference))
        {
            query += $" AND lot_loc.{ColumnNames.SiteReference} = @SiteReference";
        }

        // Add sorting
        var sortBy = request.SortBy?.ToLower() ?? "fifo";
        query += " ORDER BY ";
        switch (sortBy)
        {
            case "quantity":
                query += $"{ColumnNames.QuantityOnHand} DESC, {ColumnNames.ItemCode}, {ColumnNames.LocationCode}";
                break;
            case "location":
                query += $"{ColumnNames.LocationCode}, {ColumnNames.ItemCode}";
                break;
            case "fifo":
            default:
                query += $"FifoDate ASC, {ColumnNames.ItemCode}, {ColumnNames.LocationCode}";
                break;
        }

        var items = await _queryService.QueryAsync<StockInformationItem>(query, parameters).ConfigureAwait(false);

        return new StockInformationResponse
        {
            Items = items.ToList()
        };
    }
}


