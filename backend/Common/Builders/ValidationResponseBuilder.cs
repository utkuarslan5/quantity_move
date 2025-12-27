using quantity_move_api.Models;

namespace quantity_move_api.Common.Builders;

public static class ValidationResponseBuilder
{
    public static ItemValidationResponse ItemNotFound(string itemCode) => new()
    {
        IsValid = false,
        ItemCode = itemCode,
        IsLotTracked = false,
        ErrorMessage = "Item not found"
    };

    public static ItemValidationResponse ItemSuccess(string itemCode, bool isLotTracked, string? description = null) => new()
    {
        IsValid = true,
        ItemCode = itemCode,
        IsLotTracked = isLotTracked,
        Description = description,
        ErrorMessage = !isLotTracked ? "Item is not lot-tracked" : null
    };

    public static LotValidationResponse LotNotFound(string itemCode, string lotNumber) => new()
    {
        IsValid = false,
        ItemCode = itemCode,
        LotNumber = lotNumber,
        ErrorMessage = "Lot not found"
    };

    public static LotValidationResponse LotSuccess(string itemCode, string lotNumber) => new()
    {
        IsValid = true,
        ItemCode = itemCode,
        LotNumber = lotNumber
    };

    public static LocationValidationResponse LocationNotFound(string locationCode) => new()
    {
        IsValid = false,
        LocationCode = locationCode,
        ErrorMessage = "Location not found"
    };

    public static LocationValidationResponse LocationSuccess(string locationCode, string? locationType = null, string? description = null) => new()
    {
        IsValid = true,
        LocationCode = locationCode,
        LocationType = locationType,
        Description = description
    };

    public static StockAvailabilityResponse InsufficientStock(
        string itemCode, 
        string lotNumber, 
        string locationCode, 
        decimal availableQuantity, 
        decimal requiredQuantity) => new()
    {
        IsAvailable = false,
        ItemCode = itemCode,
        LotNumber = lotNumber,
        LocationCode = locationCode,
        AvailableQuantity = availableQuantity,
        RequiredQuantity = requiredQuantity,
        ErrorMessage = $"Insufficient stock. Available: {availableQuantity}, Required: {requiredQuantity}"
    };

    public static StockAvailabilityResponse StockAvailable(
        string itemCode, 
        string lotNumber, 
        string locationCode, 
        decimal availableQuantity, 
        decimal requiredQuantity) => new()
    {
        IsAvailable = true,
        ItemCode = itemCode,
        LotNumber = lotNumber,
        LocationCode = locationCode,
        AvailableQuantity = availableQuantity,
        RequiredQuantity = requiredQuantity
    };
}

