using System.Text.Json.Serialization;

namespace quantity_move_api.Models;

public class MoveQuantityOptions
{
    [JsonPropertyName("document_number")]
    public string? DocumentNumber { get; set; }

    [JsonPropertyName("employee_number")]
    public string? EmployeeNumber { get; set; }

    [JsonPropertyName("user_name")]
    public string? UserName { get; set; }

    [JsonPropertyName("context_type")]
    public string? ContextType { get; set; }

    [JsonPropertyName("validate_fifo")]
    public bool ValidateFifo { get; set; } = false;
}

