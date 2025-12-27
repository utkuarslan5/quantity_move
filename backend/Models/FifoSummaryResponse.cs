using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class FifoSummaryResponse
{
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("warehouse_code")]
    public string WarehouseCode { get; set; } = string.Empty;

    [JsonPropertyName("lots")]
    public List<FifoSummaryLot> Lots { get; set; } = new();
}

public class FifoSummaryLot
{
    [JsonPropertyName("lot_number")]
    public string LotNumber { get; set; } = string.Empty;

    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("quantity_on_hand")]
    public decimal QuantityOnHand { get; set; }

    [JsonPropertyName("fifo_date")]
    public DateTime FifoDate { get; set; }
}

