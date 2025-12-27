using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Common.Exceptions;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;

namespace quantity_move_api.Services.Stock;

public class StockLocationService : BaseService<StockLocationService>, IStockLocationService
{
    private readonly IQueryService _queryService;

    public StockLocationService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<StockLocationService> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public async Task<LocationsWithStockResponse> GetLocationsWithStockAsync(string itemCode, string lotNumber, string warehouseCode)
    {
        Logger.LogInformation("Getting locations with stock for item {ItemCode}, lot {LotNumber}, warehouse {WarehouseCode}", 
            itemCode, lotNumber, warehouseCode);

        var query = $@"
            SELECT 
                {ColumnNames.LocationCode} AS LocationCode,
                CAST({ColumnNames.QuantityOnHand} AS DECIMAL(18, 2)) AS QuantityOnHand,
                CONCAT({ColumnNames.LocationCode}, '-', CAST(CAST({ColumnNames.QuantityOnHand} AS DECIMAL(10, 2)) AS NVARCHAR(18))) AS LocationDisplay
            FROM {TableNames.LotLocation} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.LotNumber} = @LotNumber 
              AND {ColumnNames.QuantityOnHand} > 0 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode
            ORDER BY {ColumnNames.QuantityOnHand} DESC";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("LotNumber", lotNumber);
        parameters.Add("WarehouseCode", warehouseCode);

        var locations = await _queryService.QueryAsync<LocationWithStockItem>(query, parameters).ConfigureAwait(false);

        return new LocationsWithStockResponse
        {
            Locations = locations.ToList()
        };
    }

    public async Task<LocationsResponse> GetLocationsWithoutStockAsync(string itemCode, string lotNumber, string warehouseCode)
    {
        Logger.LogInformation("Getting locations without stock for item {ItemCode}, lot {LotNumber}, warehouse {WarehouseCode}", 
            itemCode, lotNumber, warehouseCode);

        var query = $@"
            SELECT 
                {ColumnNames.LocationCode} AS LocationCode,
                CAST({ColumnNames.QuantityOnHand} AS DECIMAL(18, 2)) AS QuantityOnHand,
                CONCAT({ColumnNames.LocationCode}, '-', CAST(CAST({ColumnNames.QuantityOnHand} AS DECIMAL(10, 2)) AS NVARCHAR(18))) AS LocationDisplay
            FROM {TableNames.LotLocation} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.LotNumber} = @LotNumber 
              AND {ColumnNames.QuantityOnHand} = 0 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode
            ORDER BY {ColumnNames.LocationCode}";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("LotNumber", lotNumber);
        parameters.Add("WarehouseCode", warehouseCode);

        var locations = await _queryService.QueryAsync<LocationItem>(query, parameters).ConfigureAwait(false);

        return new LocationsResponse
        {
            Locations = locations.ToList()
        };
    }

    public async Task<LocationDetailsResponse> GetLocationDetailsAsync(string locationCode, string? siteReference = null)
    {
        Logger.LogInformation("Getting location details for {LocationCode}, site {SiteReference}", locationCode, siteReference ?? "default");

        var query = $@"
            SELECT 
                {ColumnNames.LocationCode} AS LocationCode,
                {ColumnNames.LocationType} AS LocationType,
                {ColumnNames.Description},
                {ColumnNames.WarehouseCode} AS WarehouseCode,
                {ColumnNames.SiteReference} AS SiteReference
            FROM {TableNames.LocationMaster} 
            WHERE {ColumnNames.LocationCode} = @LocationCode";

        var parameters = new DynamicParameters();
        parameters.Add("LocationCode", locationCode);

        if (!string.IsNullOrEmpty(siteReference))
        {
            query += $" AND {ColumnNames.SiteReference} = @SiteReference";
            parameters.Add("SiteReference", siteReference);
        }

        var result = await _queryService.QueryFirstOrDefaultAsync<LocationDetailsResponse>(query, parameters).ConfigureAwait(false);

        if (result == null)
        {
            Logger.LogWarning("Location {LocationCode} not found", locationCode);
            throw new InvalidOperationException($"Location {locationCode} not found");
        }

        return result;
    }
}

