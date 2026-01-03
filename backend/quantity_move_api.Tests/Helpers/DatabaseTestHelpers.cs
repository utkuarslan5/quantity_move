using quantity_move_api.Services;

namespace quantity_move_api.Tests.Helpers;

/// <summary>
/// Helper methods for creating and cleaning up test data in the database.
/// </summary>
public static class DatabaseTestHelpers
{
    /// <summary>
    /// Creates a test item in item_mst table
    /// </summary>
    public static async Task CreateTestItemAsync(
        IDatabaseService databaseService,
        string itemCode,
        string siteRef = "faz",
        bool lotTracked = true)
    {
        var sql = @"
            IF NOT EXISTS (SELECT 1 FROM item_mst WHERE item = @ItemCode AND site_Ref = @SiteRef)
            BEGIN
                INSERT INTO item_mst (item, site_Ref, lot_tracked, item_desc)
                VALUES (@ItemCode, @SiteRef, @LotTracked, @ItemDesc)
            END";

        await databaseService.ExecuteAsync(sql, new
        {
            ItemCode = itemCode,
            SiteRef = siteRef,
            LotTracked = lotTracked ? 1 : 0,
            ItemDesc = $"Test Item {itemCode}"
        });
    }

    /// <summary>
    /// Creates a test lot in lot_mst table
    /// </summary>
    public static async Task CreateTestLotAsync(
        IDatabaseService databaseService,
        string itemCode,
        string lotNumber,
        string siteRef = "faz",
        DateTime? createDate = null)
    {
        var sql = @"
            IF NOT EXISTS (SELECT 1 FROM lot_mst WHERE item = @ItemCode AND lot = @LotNumber AND site_Ref = @SiteRef)
            BEGIN
                INSERT INTO lot_mst (item, lot, site_Ref, create_date, RecordDate)
                VALUES (@ItemCode, @LotNumber, @SiteRef, @CreateDate, @RecordDate)
            END";

        var recordDate = createDate ?? DateTime.Now;

        await databaseService.ExecuteAsync(sql, new
        {
            ItemCode = itemCode,
            LotNumber = lotNumber,
            SiteRef = siteRef,
            CreateDate = recordDate,
            RecordDate = recordDate
        });
    }

    /// <summary>
    /// Creates a test location in location_mst table
    /// </summary>
    public static async Task CreateTestLocationAsync(
        IDatabaseService databaseService,
        string locationCode,
        string siteRef = "faz",
        string? locationType = null)
    {
        var sql = @"
            IF NOT EXISTS (SELECT 1 FROM location_mst WHERE loc = @LocationCode AND site_Ref = @SiteRef)
            BEGIN
                INSERT INTO location_mst (loc, site_Ref, loc_type)
                VALUES (@LocationCode, @SiteRef, @LocationType)
            END";

        await databaseService.ExecuteAsync(sql, new
        {
            LocationCode = locationCode,
            SiteRef = siteRef,
            LocationType = locationType ?? "STOCK"
        });
    }

    /// <summary>
    /// Creates test stock in lot_loc_mst table
    /// </summary>
    public static async Task CreateTestStockAsync(
        IDatabaseService databaseService,
        string itemCode,
        string lotNumber,
        string locationCode,
        decimal quantity,
        string warehouseCode = "MAIN",
        string siteRef = "faz")
    {
        var sql = @"
            IF EXISTS (SELECT 1 FROM lot_loc_mst WHERE item = @ItemCode AND lot = @LotNumber AND loc = @LocationCode AND whse = @WarehouseCode)
            BEGIN
                UPDATE lot_loc_mst 
                SET qty_on_hand = @Quantity
                WHERE item = @ItemCode AND lot = @LotNumber AND loc = @LocationCode AND whse = @WarehouseCode
            END
            ELSE
            BEGIN
                INSERT INTO lot_loc_mst (item, lot, loc, whse, qty_on_hand, site_Ref)
                VALUES (@ItemCode, @LotNumber, @LocationCode, @WarehouseCode, @Quantity, @SiteRef)
            END";

        await databaseService.ExecuteAsync(sql, new
        {
            ItemCode = itemCode,
            LotNumber = lotNumber,
            LocationCode = locationCode,
            WarehouseCode = warehouseCode,
            Quantity = quantity,
            SiteRef = siteRef
        });
    }

    /// <summary>
    /// Creates complete test data setup (item, lot, location, stock)
    /// </summary>
    public static async Task CreateCompleteTestDataAsync(
        IDatabaseService databaseService,
        string itemCode,
        string lotNumber,
        string sourceLocation,
        string targetLocation,
        decimal sourceQuantity,
        decimal targetQuantity = 0,
        string warehouseCode = "MAIN",
        string siteRef = "faz")
    {
        await CreateTestItemAsync(databaseService, itemCode, siteRef);
        await CreateTestLotAsync(databaseService, itemCode, lotNumber, siteRef);
        await CreateTestLocationAsync(databaseService, sourceLocation, siteRef);
        await CreateTestLocationAsync(databaseService, targetLocation, siteRef);
        await CreateTestStockAsync(databaseService, itemCode, lotNumber, sourceLocation, sourceQuantity, warehouseCode, siteRef);
        
        if (targetQuantity > 0)
        {
            await CreateTestStockAsync(databaseService, itemCode, lotNumber, targetLocation, targetQuantity, warehouseCode, siteRef);
        }
    }

    /// <summary>
    /// Cleans up test stock data
    /// </summary>
    public static async Task CleanupTestStockAsync(
        IDatabaseService databaseService,
        string itemCode,
        string? lotNumber = null,
        string? locationCode = null,
        string warehouseCode = "MAIN")
    {
        var sql = "DELETE FROM lot_loc_mst WHERE item = @ItemCode AND whse = @WarehouseCode";
        object parameters;

        if (!string.IsNullOrEmpty(lotNumber) && !string.IsNullOrEmpty(locationCode))
        {
            sql += " AND lot = @LotNumber AND loc = @LocationCode";
            parameters = new { ItemCode = itemCode, LotNumber = lotNumber, LocationCode = locationCode, WarehouseCode = warehouseCode };
        }
        else if (!string.IsNullOrEmpty(lotNumber))
        {
            sql += " AND lot = @LotNumber";
            parameters = new { ItemCode = itemCode, LotNumber = lotNumber, WarehouseCode = warehouseCode };
        }
        else if (!string.IsNullOrEmpty(locationCode))
        {
            sql += " AND loc = @LocationCode";
            parameters = new { ItemCode = itemCode, LocationCode = locationCode, WarehouseCode = warehouseCode };
        }
        else
        {
            parameters = new { ItemCode = itemCode, WarehouseCode = warehouseCode };
        }

        await databaseService.ExecuteAsync(sql, parameters);
    }

    /// <summary>
    /// Cleans up test item data
    /// </summary>
    public static async Task CleanupTestItemAsync(
        IDatabaseService databaseService,
        string itemCode,
        string siteRef = "faz")
    {
        // Clean up in reverse order due to foreign key constraints
        await CleanupTestStockAsync(databaseService, itemCode);
        
        var sql = "DELETE FROM lot_mst WHERE item = @ItemCode AND site_Ref = @SiteRef";
        await databaseService.ExecuteAsync(sql, new { ItemCode = itemCode, SiteRef = siteRef });

        sql = "DELETE FROM item_mst WHERE item = @ItemCode AND site_Ref = @SiteRef";
        await databaseService.ExecuteAsync(sql, new { ItemCode = itemCode, SiteRef = siteRef });
    }

    /// <summary>
    /// Cleans up test location data
    /// </summary>
    public static async Task CleanupTestLocationAsync(
        IDatabaseService databaseService,
        string locationCode,
        string siteRef = "faz")
    {
        // Clean up stock first
        var sql = "DELETE FROM lot_loc_mst WHERE loc = @LocationCode";
        await databaseService.ExecuteAsync(sql, new { LocationCode = locationCode });

        sql = "DELETE FROM location_mst WHERE loc = @LocationCode AND site_Ref = @SiteRef";
        await databaseService.ExecuteAsync(sql, new { LocationCode = locationCode, SiteRef = siteRef });
    }

    /// <summary>
    /// Gets current stock quantity for verification
    /// </summary>
    public static async Task<decimal?> GetStockQuantityAsync(
        IDatabaseService databaseService,
        string itemCode,
        string lotNumber,
        string locationCode,
        string warehouseCode = "MAIN")
    {
        var sql = @"
            SELECT qty_on_hand 
            FROM lot_loc_mst 
            WHERE item = @ItemCode AND lot = @LotNumber AND loc = @LocationCode AND whse = @WarehouseCode";

        var result = await databaseService.QueryFirstOrDefaultAsync<decimal?>(sql, new
        {
            ItemCode = itemCode,
            LotNumber = lotNumber,
            LocationCode = locationCode,
            WarehouseCode = warehouseCode
        });

        return result;
    }

    /// <summary>
    /// Verifies that stock exists
    /// </summary>
    public static async Task<bool> StockExistsAsync(
        IDatabaseService databaseService,
        string itemCode,
        string lotNumber,
        string locationCode,
        string warehouseCode = "MAIN")
    {
        var quantity = await GetStockQuantityAsync(databaseService, itemCode, lotNumber, locationCode, warehouseCode);
        return quantity.HasValue && quantity.Value > 0;
    }
}

