using quantity_move_api.Models;

namespace quantity_move_api.Services.Fifo;

public interface IFifoService
{
    // Get oldest lot for item
    Task<FifoOldestLotResponse> GetOldestLotAsync(string itemCode, string warehouseCode, string? siteReference = null);

    // Check if current lot violates FIFO
    Task<FifoValidationResponse> ValidateFifoComplianceAsync(string itemCode, string lotNumber, string warehouseCode, string? siteReference = null);

    // Get FIFO summary for item
    Task<FifoSummaryResponse> GetFifoSummaryAsync(string itemCode, string warehouseCode, string? siteReference = null);
}

