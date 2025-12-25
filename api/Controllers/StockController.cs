using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using quantity_move_api.Models;
using quantity_move_api.Services;
using Dapper;
using System.Data;
using System.Data.SqlClient;

namespace quantity_move_api.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class StockController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<StockController> _logger;

    public StockController(
        IDatabaseService databaseService,
        IConfiguration configuration,
        ILogger<StockController> logger)
    {
        _databaseService = databaseService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("lookup")]
    public async Task<ActionResult<ApiResponse<StockLookupResponse>>> Lookup([FromBody] StockLookupRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<StockLookupResponse>.ErrorResponse("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            // Query lot_loc_mst table with filters
            var connectionString = _configuration.GetConnectionString("DefaultConnection");
            if (string.IsNullOrEmpty(connectionString))
            {
                return StatusCode(500, ApiResponse<StockLookupResponse>.ErrorResponse("Database connection not configured"));
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

            var response = new StockLookupResponse
            {
                Items = items.ToList()
            };

            return Ok(ApiResponse<StockLookupResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during stock lookup");
            return StatusCode(500, ApiResponse<StockLookupResponse>.ErrorResponse("An error occurred during stock lookup"));
        }
    }

    [HttpGet("lookup")]
    public async Task<ActionResult<ApiResponse<StockLookupResponse>>> LookupGet(
        [FromQuery] string? itemCode,
        [FromQuery] string? location,
        [FromQuery] string? warehouse,
        [FromQuery] string? site,
        [FromQuery] string? lot)
    {
        var request = new StockLookupRequest
        {
            ItemCode = itemCode,
            Location = location,
            Warehouse = warehouse,
            Site = site,
            Lot = lot
        };

        return await Lookup(request);
    }
}

