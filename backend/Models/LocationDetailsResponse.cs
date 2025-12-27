using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class LocationDetailsResponse
{
    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;

    [JsonPropertyName("location_type")]
    public string? LocationType { get; set; }

    [JsonPropertyName("description")]
    public string? Description { get; set; }

    [JsonPropertyName("warehouse_code")]
    public string? WarehouseCode { get; set; }

    [JsonPropertyName("site_reference")]
    public string? SiteReference { get; set; }
}

