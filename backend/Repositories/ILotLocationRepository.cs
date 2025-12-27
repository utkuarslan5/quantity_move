using quantity_move_api.Models;

namespace quantity_move_api.Repositories;

public interface ILotLocationRepository
{
    Task<decimal> GetQuantityAsync(string itemCode, string lotNumber, string locationCode, string warehouseCode);
    Task<IEnumerable<LocationItem>> GetLocationsAsync(string itemCode, string lotNumber, string warehouseCode, bool includeZeroQuantity = false);
    Task<IEnumerable<StockSummaryLocation>> GetStockSummaryAsync(string itemCode, string warehouseCode);
    Task<decimal?> GetAvailableQuantityAsync(string itemCode, string lotNumber, string locationCode, string warehouseCode);
}

