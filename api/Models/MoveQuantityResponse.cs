namespace quantity_move_api.Models;

public class MoveQuantityResponse
{
    public bool Success { get; set; }
    public string? TransactionId { get; set; }
    public string? Message { get; set; }
    public int? ReturnCode { get; set; }
    public int? RowsAffected { get; set; }
}

public class ValidateStockResponse
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public decimal? AvailableQuantity { get; set; }
    public int? ReturnCode { get; set; }
}

