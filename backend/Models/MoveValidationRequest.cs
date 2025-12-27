using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class MoveValidationRequest
{
    [Required]
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("lot_number")]
    public string LotNumber { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("source_location")]
    public string SourceLocation { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("target_location")]
    public string TargetLocation { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("warehouse_code")]
    public string? WarehouseCode { get; set; }
}

