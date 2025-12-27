using quantity_move_api.Models;

namespace quantity_move_api.Services;

public interface IStockService
{
    // Stored Procedure Operations
    Task<StockValidationResponse> ValidateStockAsync(StockValidationRequest request);

    // Query Operations
    Task<QuantityOnHandResponse> GetQuantityOnHandAsync(QuantityOnHandRequest request);
    Task<StockInformationResponse> GetStockInformationAsync(StockInformationRequest request);
}



