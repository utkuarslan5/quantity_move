using quantity_move_api.Models;

namespace quantity_move_api.Services.Stock;

public interface IStockValidationService
{
    // Validate item exists and is lot-tracked
    Task<ItemValidationResponse> ValidateItemAsync(string itemCode, string? siteReference = null);

    // Validate lot exists
    Task<LotValidationResponse> ValidateLotAsync(string itemCode, string lotNumber);

    // Validate location exists and is valid
    Task<LocationValidationResponse> ValidateLocationAsync(string locationCode, string? siteReference = null);

    // Validate stock availability at location
    Task<StockAvailabilityResponse> ValidateStockAvailabilityAsync(string itemCode, string lotNumber, string locationCode, decimal requiredQuantity, string warehouseCode);

    // Combined validation (composes above methods)
    Task<CombinedValidationResponse> ValidateStockForMoveAsync(StockMoveValidationRequest request);
}

