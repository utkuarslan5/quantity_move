using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class MoveQuantityRequest
{
    [Required]
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("source_location")]
    public string SourceLocation { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("source_lot_number")]
    public string SourceLotNumber { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("target_location")]
    public string TargetLocation { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    [JsonPropertyName("quantity")]
    public decimal Quantity { get; set; }

    [JsonPropertyName("warehouse_code")]
    public string? WarehouseCode { get; set; }

    [JsonPropertyName("site_reference")]
    public string? SiteReference { get; set; }

    [JsonPropertyName("document_number")]
    public string? DocumentNumber { get; set; }
}

