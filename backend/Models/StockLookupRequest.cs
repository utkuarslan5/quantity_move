namespace quantity_move_api.Models;

public class StockLookupRequest
{
    public string? ItemCode { get; set; }
    public string? Location { get; set; }
    public string? Warehouse { get; set; }
    public string? Site { get; set; }
    public string? Lot { get; set; }
}

