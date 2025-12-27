using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class GetLocationsRequest
{
    [Required]
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("lot_number")]
    public string LotNumber { get; set; } = string.Empty;

    [JsonPropertyName("warehouse_code")]
    public string? WarehouseCode { get; set; }

    [JsonPropertyName("include_zero_quantity")]
    public bool IncludeZeroQuantity { get; set; } = false;
}

