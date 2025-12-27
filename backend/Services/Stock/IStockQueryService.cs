using quantity_move_api.Models;

namespace quantity_move_api.Services.Stock;

public interface IStockQueryService
{
    // Get quantity at specific location
    Task<decimal> GetQuantityAtLocationAsync(string itemCode, string lotNumber, string locationCode, string warehouseCode);

    // Get all locations for item/lot (with or without stock)
    Task<LocationsResponse> GetLocationsForItemLotAsync(string itemCode, string lotNumber, string warehouseCode, bool includeZeroQuantity = false);

    // Get stock summary for item
    Task<StockSummaryResponse> GetStockSummaryAsync(string itemCode, string warehouseCode);

    // Get current location(s) for item/lot
    Task<CurrentLocationResponse> GetCurrentLocationAsync(string itemCode, string lotNumber, string warehouseCode);
}

