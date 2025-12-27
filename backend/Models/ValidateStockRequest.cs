using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class ValidateStockRequest
{
    [Required]
    [JsonPropertyName("item_code")]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("lot_number")]
    public string LotNumber { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("location_code")]
    public string LocationCode { get; set; } = string.Empty;

    [Required]
    [JsonPropertyName("required_quantity")]
    public decimal RequiredQuantity { get; set; }

    [JsonPropertyName("warehouse_code")]
    public string? WarehouseCode { get; set; }
}

