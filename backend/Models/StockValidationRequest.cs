using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class StockValidationRequest
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
    
    [JsonPropertyName("warehouse_code")]
    public string? WarehouseCode { get; set; }
    
    [JsonPropertyName("site_reference")]
    public string? SiteReference { get; set; }
    
    [JsonPropertyName("validation_date")]
    public DateTime? ValidationDate { get; set; }
    
    [JsonPropertyName("check_quantity_available")]
    public bool CheckQuantityAvailable { get; set; } = true;
    
    [JsonPropertyName("required_quantity")]
    public decimal? RequiredQuantity { get; set; }
    
    [JsonPropertyName("return_location_info")]
    public bool ReturnLocationInfo { get; set; } = false;  // NEW: Get location details
}

