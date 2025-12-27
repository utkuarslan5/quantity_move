using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class CombinedValidationResponse
{
    [JsonPropertyName("is_valid")]
    public bool IsValid { get; set; }

    [JsonPropertyName("item_validation")]
    public ItemValidationResponse? ItemValidation { get; set; }

    [JsonPropertyName("lot_validation")]
    public LotValidationResponse? LotValidation { get; set; }

    [JsonPropertyName("source_location_validation")]
    public LocationValidationResponse? SourceLocationValidation { get; set; }

    [JsonPropertyName("target_location_validation")]
    public LocationValidationResponse? TargetLocationValidation { get; set; }

    [JsonPropertyName("stock_availability")]
    public StockAvailabilityResponse? StockAvailability { get; set; }

    [JsonPropertyName("error_message")]
    public string? ErrorMessage { get; set; }
}

