using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class LocationsResponse
{
    [JsonPropertyName("locations")]
    public List<LocationItem> Locations { get; set; } = new();
}

public class LocationItem
{
    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("quantity_on_hand")]
    public decimal QuantityOnHand { get; set; }

    [JsonPropertyName("location_display")]
    public string? LocationDisplay { get; set; }
}

