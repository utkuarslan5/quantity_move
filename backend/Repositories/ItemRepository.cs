using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Repositories;
using quantity_move_api.Services;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;

namespace quantity_move_api.Repositories;

public class ItemRepository : BaseService<ItemRepository>, IItemRepository
{
    private readonly IQueryService _queryService;

    public ItemRepository(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<ItemRepository> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public async Task<ItemValidationResponse?> GetItemAsync(string itemCode, string? siteReference = null)
    {
        Logger.LogInformation("Getting item {ItemCode}, site {SiteReference}", itemCode, siteReference ?? "default");

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

        return await _queryService.QueryFirstOrDefaultAsync<ItemValidationResponse>(query, parameters).ConfigureAwait(false);
    }

    public async Task<bool> IsLotTrackedAsync(string itemCode, string? siteReference = null)
    {
        Logger.LogInformation("Checking if item {ItemCode} is lot-tracked, site {SiteReference}", itemCode, siteReference ?? "default");

        var query = $@"
            SELECT {ColumnNames.LotTracked}
            FROM {TableNames.ItemMaster} 
            WHERE {ColumnNames.ItemCode} = @ItemCode";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);

        if (!string.IsNullOrEmpty(siteReference))
        {
            query += $" AND {ColumnNames.SiteReference} = @SiteReference";
            parameters.Add("SiteReference", siteReference);
        }

        var result = await _queryService.QueryFirstOrDefaultAsync<bool?>(query, parameters).ConfigureAwait(false);
        return result ?? false;
    }

    public async Task<bool> ItemExistsAsync(string itemCode, string? siteReference = null)
    {
        Logger.LogInformation("Checking if item {ItemCode} exists, site {SiteReference}", itemCode, siteReference ?? "default");

        var query = $@"
            SELECT COUNT(*) 
            FROM {TableNames.ItemMaster} 
            WHERE {ColumnNames.ItemCode} = @ItemCode";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", itemCode);

        if (!string.IsNullOrEmpty(siteReference))
        {
            query += $" AND {ColumnNames.SiteReference} = @SiteReference";
            parameters.Add("SiteReference", siteReference);
        }

        var count = await _queryService.QueryFirstOrDefaultAsync<int>(query, parameters).ConfigureAwait(false);
        return count > 0;
    }
}

