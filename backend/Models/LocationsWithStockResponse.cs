using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class LocationsWithStockResponse
{
    [JsonPropertyName("locations")]
    public List<LocationWithStockItem> Locations { get; set; } = new();
}

public class LocationWithStockItem
{
    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("quantity_on_hand")]
    public decimal QuantityOnHand { get; set; }

    [JsonPropertyName("location_display")]
    public string LocationDisplay { get; set; } = string.Empty;
}

