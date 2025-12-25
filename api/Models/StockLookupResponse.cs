namespace quantity_move_api.Models;

public class StockLookupResponse
{
    public List<StockItem> Items { get; set; } = new();
}

public class StockItem
{
    public string ItemCode { get; set; } = string.Empty;
    public string? Location { get; set; }
    public string? Warehouse { get; set; }
    public string? Site { get; set; }
    public string? Lot { get; set; }
    public decimal Quantity { get; set; }
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ProductionDate { get; set; }
    public int? Priority { get; set; } // For FIFO ordering
}

