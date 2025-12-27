using quantity_move_api.Models;

namespace quantity_move_api.Services;

public interface IStockService
{
    Task<StockLookupResponse> GetStockAsync(StockLookupRequest request);
}



