using System.ComponentModel.DataAnnotations;

namespace quantity_move_api.Models;

public class MoveQuantityRequest
{
    [Required]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    public string SourceLocation { get; set; } = string.Empty;

    [Required]
    public string TargetLocation { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }

    public string? Lot { get; set; }
    public string? Warehouse { get; set; }
    public string? Site { get; set; }
    public string? UserId { get; set; }
}

public class MoveQuantityMultiRequest
{
    [Required]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    public string SourceLocation { get; set; } = string.Empty;

    [Required]
    public List<MultiWarehouseTarget> Targets { get; set; } = new();

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Total quantity must be greater than 0")]
    public decimal TotalQuantity { get; set; }

    public string? Lot { get; set; }
    public string? UserId { get; set; }
}

public class MultiWarehouseTarget
{
    [Required]
    public string TargetLocation { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }

    public string? Warehouse { get; set; }
    public string? Site { get; set; }
}

public class ValidateStockRequest
{
    [Required]
    public string ItemCode { get; set; } = string.Empty;

    [Required]
    public string Location { get; set; } = string.Empty;

    [Required]
    [Range(0.01, double.MaxValue, ErrorMessage = "Quantity must be greater than 0")]
    public decimal Quantity { get; set; }

    public string? Lot { get; set; }
    public string? Warehouse { get; set; }
    public string? Site { get; set; }
}

