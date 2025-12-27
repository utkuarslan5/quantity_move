using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;

namespace quantity_move_api.Services.Stock;

public class StockQueryService : BaseService<StockQueryService>, IStockQueryService
{
    private readonly IQueryService _queryService;

    public StockQueryService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<StockQueryService> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public async Task<decimal> GetQuantityAtLocationAsync(string itemCode, string lotNumber, string locationCode, string warehouseCode)
    {
        Logger.LogInformation("Getting quantity for item {ItemCode}, lot {LotNumber}, location {LocationCode}, warehouse {WarehouseCode}", 
            itemCode, lotNumber, locationCode, warehouseCode);

        var query = $@"
            SELECT {ColumnNames.QuantityOnHand} 
            FROM {TableNames.LotLocation} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.LocationCode} = @LocationCode 
              AND {ColumnNames.LotNumber} = @LotNumber 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("LocationCode", locationCode);
        parameters.Add("LotNumber", lotNumber);
        parameters.Add("WarehouseCode", warehouseCode);

        var result = await _queryService.QueryFirstOrDefaultAsync<decimal?>(query, parameters).ConfigureAwait(false);
        return result ?? 0;
    }

    public async Task<LocationsResponse> GetLocationsForItemLotAsync(string itemCode, string lotNumber, string warehouseCode, bool includeZeroQuantity = false)
    {
        Logger.LogInformation("Getting locations for item {ItemCode}, lot {LotNumber}, warehouse {WarehouseCode}, includeZero: {IncludeZero}", 
            itemCode, lotNumber, warehouseCode, includeZeroQuantity);

        var query = $@"
            SELECT 
                {ColumnNames.LocationCode} AS LocationCode,
                CAST({ColumnNames.QuantityOnHand} AS DECIMAL(18, 2)) AS QuantityOnHand,
                CONCAT({ColumnNames.LocationCode}, '-', CAST(CAST({ColumnNames.QuantityOnHand} AS DECIMAL(10, 2)) AS NVARCHAR(18))) AS LocationDisplay
            FROM {TableNames.LotLocation} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.LotNumber} = @LotNumber 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode";

        if (!includeZeroQuantity)
        {
            query += $" AND {ColumnNames.QuantityOnHand} > 0";
        }

        query += $" ORDER BY {ColumnNames.QuantityOnHand} DESC";

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

    public async Task<StockSummaryResponse> GetStockSummaryAsync(string itemCode, string warehouseCode)
    {
        Logger.LogInformation("Getting stock summary for item {ItemCode}, warehouse {WarehouseCode}", itemCode, warehouseCode);

        var query = $@"
            SELECT 
                {ColumnNames.LocationCode} AS LocationCode,
                {ColumnNames.LotNumber} AS LotNumber,
                CAST({ColumnNames.QuantityOnHand} AS DECIMAL(18, 2)) AS QuantityOnHand
            FROM {TableNames.LotLocation} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode
              AND {ColumnNames.QuantityOnHand} > 0
            ORDER BY {ColumnNames.LocationCode}, {ColumnNames.LotNumber}";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("WarehouseCode", warehouseCode);

        var locations = await _queryService.QueryAsync<StockSummaryLocation>(query, parameters).ConfigureAwait(false);
        var locationsList = locations.ToList();

        return new StockSummaryResponse
        {
            ItemCode = itemCode,
            WarehouseCode = warehouseCode,
            TotalQuantity = locationsList.Sum(l => l.QuantityOnHand),
            LocationCount = locationsList.Count,
            Locations = locationsList
        };
    }

    public async Task<CurrentLocationResponse> GetCurrentLocationAsync(string itemCode, string lotNumber, string warehouseCode)
    {
        Logger.LogInformation("Getting current location for item {ItemCode}, lot {LotNumber}, warehouse {WarehouseCode}", 
            itemCode, lotNumber, warehouseCode);

        var query = $@"
            SELECT 
                {ColumnNames.LocationCode} AS LocationCode,
                CAST({ColumnNames.QuantityOnHand} AS DECIMAL(18, 2)) AS QuantityOnHand
            FROM {TableNames.LotLocation} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.LotNumber} = @LotNumber 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode
            ORDER BY {ColumnNames.QuantityOnHand} DESC";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("LotNumber", lotNumber);
        parameters.Add("WarehouseCode", warehouseCode);

        var locations = await _queryService.QueryAsync<CurrentLocationItem>(query, parameters).ConfigureAwait(false);

        return new CurrentLocationResponse
        {
            ItemCode = itemCode,
            LotNumber = lotNumber,
            Locations = locations.ToList()
        };
    }
}

