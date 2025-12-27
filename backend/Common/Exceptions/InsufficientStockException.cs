namespace quantity_move_api.Common.Exceptions;

public class InsufficientStockException : Exception
{
    public string ItemCode { get; }
    public string LocationCode { get; }
    public decimal AvailableQuantity { get; }
    public decimal RequiredQuantity { get; }

    public InsufficientStockException(
        string itemCode, 
        string locationCode, 
        decimal availableQuantity, 
        decimal requiredQuantity)
        : base($"Insufficient stock for item '{itemCode}' at location '{locationCode}'. Available: {availableQuantity}, Required: {requiredQuantity}")
    {
        ItemCode = itemCode;
        LocationCode = locationCode;
        AvailableQuantity = availableQuantity;
        RequiredQuantity = requiredQuantity;
    }
}

