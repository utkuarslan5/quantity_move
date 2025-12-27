using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class ValidateTargetRequest
{
    [Required]
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("target_location")]
    public string TargetLocation { get; set; } = string.Empty;

    [JsonPropertyName("warehouse_code")]
    public string? WarehouseCode { get; set; }
}

