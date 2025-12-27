using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;

namespace quantity_move_api.Services.Fifo;

public class FifoService : BaseService<FifoService>, IFifoService
{
    private readonly IQueryService _queryService;

    public FifoService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<FifoService> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public async Task<FifoOldestLotResponse> GetOldestLotAsync(string itemCode, string warehouseCode, string? siteReference = null)
    {
        Logger.LogInformation("Getting oldest lot for item {ItemCode}, warehouse {WarehouseCode}, site {SiteReference}", 
            itemCode, warehouseCode, siteReference ?? "default");

        var query = $@"
            SELECT TOP 1
                {ColumnNames.ItemCode} AS ItemCode,
                {ColumnNames.LocationCode} AS LocationCode,
                {ColumnNames.LotNumber} AS OldestLotNumber,
                {ColumnNames.QuantityOnHand} AS QuantityOnHand,
                {ColumnNames.FifoDate} AS FifoDate
            FROM {TableNames.FifoSummary} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("WarehouseCode", warehouseCode);

        if (!string.IsNullOrEmpty(siteReference))
        {
            query += $" AND {ColumnNames.SiteReference} = @SiteReference";
            parameters.Add("SiteReference", siteReference);
        }
        else
        {
            query += $" AND (@SiteReference IS NULL OR {ColumnNames.SiteReference} IS NULL)";
            parameters.Add("SiteReference", (object)DBNull.Value);
        }

        query += $" ORDER BY {ColumnNames.FifoDate} ASC";

        var result = await _queryService.QueryFirstOrDefaultAsync<FifoOldestLotResponse>(query, parameters).ConfigureAwait(false);

        return result ?? new FifoOldestLotResponse
        {
            ItemCode = itemCode
        };
    }

    public async Task<FifoValidationResponse> ValidateFifoComplianceAsync(string itemCode, string lotNumber, string warehouseCode, string? siteReference = null)
    {
        var oldestLot = await GetOldestLotAsync(itemCode, warehouseCode, siteReference);

        if (string.IsNullOrEmpty(oldestLot.OldestLotNumber))
        {
            return new FifoValidationResponse
            {
                IsCompliant = true,
                ItemCode = itemCode,
                CurrentLotNumber = lotNumber
            };
        }

        var isCompliant = oldestLot.OldestLotNumber == lotNumber;
        var warningMessage = isCompliant 
            ? null 
            : $"Older lot exists: {oldestLot.OldestLotNumber} at location {oldestLot.LocationCode} (FIFO date: {oldestLot.FifoDate:yyyy-MM-dd})";

        return new FifoValidationResponse
        {
            IsCompliant = isCompliant,
            ItemCode = itemCode,
            CurrentLotNumber = lotNumber,
            OldestLotNumber = oldestLot.OldestLotNumber,
            WarningMessage = warningMessage
        };
    }

    public async Task<FifoSummaryResponse> GetFifoSummaryAsync(string itemCode, string warehouseCode, string? siteReference = null)
    {
        Logger.LogInformation("Getting FIFO summary for item {ItemCode}, warehouse {WarehouseCode}, site {SiteReference}", 
            itemCode, warehouseCode, siteReference ?? "default");

        var query = $@"
            SELECT 
                {ColumnNames.LotNumber} AS LotNumber,
                {ColumnNames.LocationCode} AS LocationCode,
                {ColumnNames.QuantityOnHand} AS QuantityOnHand,
                {ColumnNames.FifoDate} AS FifoDate
            FROM {TableNames.FifoSummary} 
            WHERE {ColumnNames.ItemCode} = @ItemCode 
              AND {ColumnNames.WarehouseCode} = @WarehouseCode";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);
        parameters.Add("WarehouseCode", warehouseCode);

        if (!string.IsNullOrEmpty(siteReference))
        {
            query += $" AND {ColumnNames.SiteReference} = @SiteReference";
            parameters.Add("SiteReference", siteReference);
        }

        query += $" ORDER BY {ColumnNames.FifoDate} ASC";

        var lots = await _queryService.QueryAsync<FifoSummaryLot>(query, parameters).ConfigureAwait(false);

        return new FifoSummaryResponse
        {
            ItemCode = itemCode,
            WarehouseCode = warehouseCode,
            Lots = lots.ToList()
        };
    }
}

