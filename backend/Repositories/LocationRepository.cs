using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Repositories;
using quantity_move_api.Services;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;

namespace quantity_move_api.Repositories;

public class LocationRepository : BaseService<LocationRepository>, ILocationRepository
{
    private readonly IQueryService _queryService;

    public LocationRepository(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<LocationRepository> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public async Task<LocationValidationResponse?> GetLocationAsync(string locationCode, string? siteReference = null)
    {
        Logger.LogInformation("Getting location {LocationCode}, site {SiteReference}", locationCode, siteReference ?? "default");

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

        return await _queryService.QueryFirstOrDefaultAsync<LocationValidationResponse>(query, parameters).ConfigureAwait(false);
    }

    public async Task<LocationDetailsResponse?> GetLocationDetailsAsync(string locationCode, string? siteReference = null)
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

        return await _queryService.QueryFirstOrDefaultAsync<LocationDetailsResponse>(query, parameters).ConfigureAwait(false);
    }

    public async Task<bool> LocationExistsAsync(string locationCode, string? siteReference = null)
    {
        Logger.LogInformation("Checking if location {LocationCode} exists, site {SiteReference}", locationCode, siteReference ?? "default");

        var query = $@"
            SELECT COUNT(*) 
            FROM {TableNames.LocationMaster} 
            WHERE {ColumnNames.LocationCode} = @LocationCode";

        var parameters = new DynamicParameters();
        parameters.Add("LocationCode", locationCode);

        if (!string.IsNullOrEmpty(siteReference))
        {
            query += $" AND {ColumnNames.SiteReference} = @SiteReference";
            parameters.Add("SiteReference", siteReference);
        }

        var count = await _queryService.QueryFirstOrDefaultAsync<int>(query, parameters).ConfigureAwait(false);
        return count > 0;
    }
}

