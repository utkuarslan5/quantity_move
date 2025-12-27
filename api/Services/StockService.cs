using Dapper;
using quantity_move_api.Models;
using System.Data.SqlClient;

namespace quantity_move_api.Services;

public class StockService : IStockService
{
    private readonly IConfiguration _configuration;
    private readonly ILogger<StockService> _logger;

    public StockService(IConfiguration configuration, ILogger<StockService> logger)
    {
        _configuration = configuration;
        _logger = logger;
    }

    public async Task<StockLookupResponse> GetStockAsync(StockLookupRequest request)
    {
        var connectionString = _configuration.GetConnectionString("DefaultConnection");
        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException("Database connection not configured");
        }

        using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync();

        var query = @"
            SELECT 
                item AS ItemCode,
                loc AS Location,
                whse AS Warehouse,
                site AS Site,
                lot AS Lot,
                qty_on_hand AS Quantity,
                exp_date AS ExpiryDate,
                prod_date AS ProductionDate,
                ROW_NUMBER() OVER (ORDER BY prod_date ASC, exp_date ASC) AS Priority
            FROM lot_loc_mst
            WHERE 1=1";

        var parameters = new DynamicParameters();

        if (!string.IsNullOrEmpty(request.ItemCode))
        {
            query += " AND item = @ItemCode";
            parameters.Add("ItemCode", request.ItemCode);
        }

        if (!string.IsNullOrEmpty(request.Location))
        {
            query += " AND loc = @Location";
            parameters.Add("Location", request.Location);
        }

        if (!string.IsNullOrEmpty(request.Warehouse))
        {
            query += " AND whse = @Warehouse";
            parameters.Add("Warehouse", request.Warehouse);
        }

        if (!string.IsNullOrEmpty(request.Site))
        {
            query += " AND site = @Site";
            parameters.Add("Site", request.Site);
        }

        if (!string.IsNullOrEmpty(request.Lot))
        {
            query += " AND lot = @Lot";
            parameters.Add("Lot", request.Lot);
        }

        query += " ORDER BY prod_date ASC, exp_date ASC";

        var items = await connection.QueryAsync<StockItem>(query, parameters);

        return new StockLookupResponse
        {
            Items = items.ToList()
        };
    }
}



