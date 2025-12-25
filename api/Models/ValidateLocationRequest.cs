using System.ComponentModel.DataAnnotations;

namespace quantity_move_api.Models;

public class ValidateLocationRequest
{
    [Required]
    public string LocationCode { get; set; } = string.Empty;

    public string? Warehouse { get; set; }
    public string? Site { get; set; }
}

public class ValidateLocationResponse
{
    public bool IsValid { get; set; }
    public string? Message { get; set; }
    public string? LocationCode { get; set; }
    public string? LocationDescription { get; set; }
}

