using quantity_move_api.Models;

namespace quantity_move_api.Repositories;

public interface IItemRepository
{
    Task<ItemValidationResponse?> GetItemAsync(string itemCode, string? siteReference = null);
    Task<bool> IsLotTrackedAsync(string itemCode, string? siteReference = null);
    Task<bool> ItemExistsAsync(string itemCode, string? siteReference = null);
}

