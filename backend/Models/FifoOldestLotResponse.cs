using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class FifoOldestLotResponse
{
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [JsonPropertyName("oldest_lot_number")]
    public string? OldestLotNumber { get; set; }

    [JsonPropertyName("location_code")]
    public string? LocationCode { get; set; }

    [JsonPropertyName("quantity_on_hand")]
    public decimal? QuantityOnHand { get; set; }

    [JsonPropertyName("fifo_date")]
    public DateTime? FifoDate { get; set; }
}

