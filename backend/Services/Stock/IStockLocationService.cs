using quantity_move_api.Models;

namespace quantity_move_api.Services.Stock;

public interface IStockLocationService
{
    // Get locations with stock for item/lot
    Task<LocationsWithStockResponse> GetLocationsWithStockAsync(string itemCode, string lotNumber, string warehouseCode);

    // Get locations without stock (for item/lot that exists but qty = 0)
    Task<LocationsResponse> GetLocationsWithoutStockAsync(string itemCode, string lotNumber, string warehouseCode);

    // Get location details
    Task<LocationDetailsResponse> GetLocationDetailsAsync(string locationCode, string? siteReference = null);
}

