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
public class QuantityController : ControllerBase
{
    private readonly IDatabaseService _databaseService;
    private readonly IConfiguration _configuration;
    private readonly ILogger<QuantityController> _logger;

    public QuantityController(
        IDatabaseService databaseService,
        IConfiguration configuration,
        ILogger<QuantityController> logger)
    {
        _databaseService = databaseService;
        _configuration = configuration;
        _logger = logger;
    }

    [HttpPost("validate-stock")]
    public async Task<ActionResult<ApiResponse<ValidateStockResponse>>> ValidateStock([FromBody] ValidateStockRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<ValidateStockResponse>.ErrorResponse("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var procedureName = _configuration["StoredProcedures:StockValidation"] ?? "TR_Stok_Kontrol";

            // Build parameters for stored procedure
            // Adjust parameter names based on actual stored procedure signature
            var parameters = new DynamicParameters();
            parameters.Add("@item", request.ItemCode);
            parameters.Add("@loc", request.Location);
            parameters.Add("@qty", request.Quantity);
            
            if (!string.IsNullOrEmpty(request.Lot))
                parameters.Add("@lot", request.Lot);
            if (!string.IsNullOrEmpty(request.Warehouse))
                parameters.Add("@whse", request.Warehouse);
            if (!string.IsNullOrEmpty(request.Site))
                parameters.Add("@site", request.Site);

            // Add output parameters (adjust based on actual SP signature)
            parameters.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            parameters.Add("@availableQty", dbType: DbType.Decimal, direction: ParameterDirection.Output);
            parameters.Add("@message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

            await _databaseService.ExecuteStoredProcedureNonQueryAsync(procedureName, parameters);

            var returnValue = parameters.Get<int>("@returnValue");
            var availableQty = parameters.Get<decimal?>("@availableQty");
            var message = parameters.Get<string>("@message");

            var response = new ValidateStockResponse
            {
                IsValid = returnValue == 0, // Adjust based on actual return code meaning
                ReturnCode = returnValue,
                AvailableQuantity = availableQty,
                Message = message
            };

            return Ok(ApiResponse<ValidateStockResponse>.SuccessResponse(response));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during stock validation");
            return StatusCode(500, ApiResponse<ValidateStockResponse>.ErrorResponse("An error occurred during stock validation"));
        }
    }

    [HttpPost("move")]
    public async Task<ActionResult<ApiResponse<MoveQuantityResponse>>> Move([FromBody] MoveQuantityRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.ErrorResponse("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var procedureName = _configuration["StoredProcedures:MoveQuantity"] ?? "TR_Miktar_Ilerlet";

            // Build parameters for stored procedure
            // Adjust parameter names based on actual stored procedure signature
            var parameters = new DynamicParameters();
            parameters.Add("@item", request.ItemCode);
            parameters.Add("@sourceLoc", request.SourceLocation);
            parameters.Add("@targetLoc", request.TargetLocation);
            parameters.Add("@qty", request.Quantity);
            
            if (!string.IsNullOrEmpty(request.Lot))
                parameters.Add("@lot", request.Lot);
            if (!string.IsNullOrEmpty(request.Warehouse))
                parameters.Add("@whse", request.Warehouse);
            if (!string.IsNullOrEmpty(request.Site))
                parameters.Add("@site", request.Site);
            if (!string.IsNullOrEmpty(request.UserId))
                parameters.Add("@userId", request.UserId);

            // Add output parameters (adjust based on actual SP signature)
            parameters.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
            parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);
            parameters.Add("@transactionId", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
            parameters.Add("@message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

            await _databaseService.ExecuteStoredProcedureNonQueryAsync(procedureName, parameters);

            var returnValue = parameters.Get<int>("@returnValue");
            var rowsAffected = parameters.Get<int?>("@rowsAffected");
            var transactionId = parameters.Get<string>("@transactionId");
            var message = parameters.Get<string>("@message");

            var response = new MoveQuantityResponse
            {
                Success = returnValue == 0, // Adjust based on actual return code meaning
                ReturnCode = returnValue,
                RowsAffected = rowsAffected,
                TransactionId = transactionId,
                Message = message
            };

            if (!response.Success)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, message ?? "Move operation failed"));
            }

            return Ok(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, "Move operation completed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during quantity move");
            return StatusCode(500, ApiResponse<MoveQuantityResponse>.ErrorResponse("An error occurred during quantity move"));
        }
    }

    [HttpPost("move-multi")]
    public async Task<ActionResult<ApiResponse<MoveQuantityResponse>>> MoveMulti([FromBody] MoveQuantityMultiRequest request)
    {
        try
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.ErrorResponse("Invalid request",
                    ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList()));
            }

            var procedureName = _configuration["StoredProcedures:MoveQuantityMulti"] ?? "TR_Miktar_Ilerlet_MultiWhse";

            // For multi-warehouse move, we may need to call the procedure multiple times or pass a table-valued parameter
            // This implementation assumes the procedure can handle multiple targets
            // Adjust based on actual stored procedure signature

            var results = new List<MoveQuantityResponse>();

            foreach (var target in request.Targets)
            {
                var parameters = new DynamicParameters();
                parameters.Add("@item", request.ItemCode);
                parameters.Add("@sourceLoc", request.SourceLocation);
                parameters.Add("@targetLoc", target.TargetLocation);
                parameters.Add("@qty", target.Quantity);
                
                if (!string.IsNullOrEmpty(request.Lot))
                    parameters.Add("@lot", request.Lot);
                if (!string.IsNullOrEmpty(target.Warehouse))
                    parameters.Add("@whse", target.Warehouse);
                if (!string.IsNullOrEmpty(target.Site))
                    parameters.Add("@site", target.Site);
                if (!string.IsNullOrEmpty(request.UserId))
                    parameters.Add("@userId", request.UserId);

                // Add output parameters
                parameters.Add("@returnValue", dbType: DbType.Int32, direction: ParameterDirection.ReturnValue);
                parameters.Add("@rowsAffected", dbType: DbType.Int32, direction: ParameterDirection.Output);
                parameters.Add("@transactionId", dbType: DbType.String, size: 100, direction: ParameterDirection.Output);
                parameters.Add("@message", dbType: DbType.String, size: 500, direction: ParameterDirection.Output);

                await _databaseService.ExecuteStoredProcedureNonQueryAsync(procedureName, parameters);

                var returnValue = parameters.Get<int>("@returnValue");
                var rowsAffected = parameters.Get<int?>("@rowsAffected");
                var transactionId = parameters.Get<string>("@transactionId");
                var message = parameters.Get<string>("@message");

                results.Add(new MoveQuantityResponse
                {
                    Success = returnValue == 0,
                    ReturnCode = returnValue,
                    RowsAffected = rowsAffected,
                    TransactionId = transactionId,
                    Message = message
                });
            }

            // Return the first result or aggregate results
            var response = results.FirstOrDefault() ?? new MoveQuantityResponse { Success = false };
            response.Success = results.All(r => r.Success);

            if (!response.Success)
            {
                return BadRequest(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, "One or more move operations failed"));
            }

            return Ok(ApiResponse<MoveQuantityResponse>.SuccessResponse(response, "Multi-warehouse move operation completed successfully"));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during multi-warehouse quantity move");
            return StatusCode(500, ApiResponse<MoveQuantityResponse>.ErrorResponse("An error occurred during multi-warehouse quantity move"));
        }
    }
}

