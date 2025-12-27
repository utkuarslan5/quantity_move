using Dapper;
using quantity_move_api.Common.Constants;
using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Query;

namespace quantity_move_api.Services.Barcode;

public class BarcodeService : BaseService<BarcodeService>, IBarcodeService
{
    private readonly IQueryService _queryService;

    public BarcodeService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IQueryService queryService,
        ILogger<BarcodeService> logger)
        : base(databaseService, configurationService, logger)
    {
        _queryService = queryService;
    }

    public Task<BarcodeParseResponse> ParseBarcodeAsync(string barcode)
    {
        if (string.IsNullOrWhiteSpace(barcode))
        {
            return Task.FromResult(new BarcodeParseResponse
            {
                IsValid = false,
                ErrorMessage = "Barcode is empty"
            });
        }

        var parts = barcode.Split('%');
        
        if (parts.Length < 2)
        {
            return Task.FromResult(new BarcodeParseResponse
            {
                IsValid = false,
                ErrorMessage = "Invalid barcode format. Expected format: item%lot%quantity"
            });
        }

        var response = new BarcodeParseResponse
        {
            ItemCode = parts[0],
            LotNumber = parts[1],
            IsValid = true
        };

        if (parts.Length >= 3 && decimal.TryParse(parts[2], out var quantity))
        {
            response.Quantity = quantity;
        }

        return Task.FromResult(response);
    }

    public async Task<BarcodeLookupResponse> LookupBarcodeAsync(string barcode)
    {
        // First parse the barcode
        var parseResult = await ParseBarcodeAsync(barcode).ConfigureAwait(false);
        
        if (!parseResult.IsValid || string.IsNullOrEmpty(parseResult.ItemCode))
        {
            return new BarcodeLookupResponse
            {
                Found = false,
                ErrorMessage = parseResult.ErrorMessage ?? "Invalid barcode format"
            };
        }

        // Lookup item information
        Logger.LogInformation("Looking up barcode {Barcode}, parsed item: {ItemCode}", barcode, parseResult.ItemCode);

        var query = $@"
            SELECT 
                {ColumnNames.ItemCode} AS ItemCode,
                {ColumnNames.Description}
            FROM {TableNames.ItemMaster} 
            WHERE {ColumnNames.ItemCode} = @ItemCode";

        var parameters = new DynamicParameters();
        parameters.Add("ItemCode", parseResult.ItemCode);

        var result = await _queryService.QueryFirstOrDefaultAsync<BarcodeLookupResponse>(query, parameters).ConfigureAwait(false);

        if (result == null)
        {
            return new BarcodeLookupResponse
            {
                Found = false,
                ItemCode = parseResult.ItemCode,
                LotNumber = parseResult.LotNumber,
                ErrorMessage = "Item not found in database"
            };
        }

        result.Found = true;
        result.LotNumber = parseResult.LotNumber;
        return result;
    }
}

