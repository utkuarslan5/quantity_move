using quantity_move_api.Models;
using quantity_move_api.Services.Base;
using quantity_move_api.Services.Stock;
using quantity_move_api.Services.Quantity;
using quantity_move_api.Services.Fifo;

namespace quantity_move_api.Services.Composition;

public class StockOperationService : BaseService<StockOperationService>, IStockOperationService
{
    private readonly IStockValidationService _validationService;
    private readonly IQuantityMoveService _moveService;
    private readonly IFifoService _fifoService;

    public StockOperationService(
        IDatabaseService databaseService,
        IConfigurationService configurationService,
        IStockValidationService validationService,
        IQuantityMoveService moveService,
        IFifoService fifoService,
        ILogger<StockOperationService> logger)
        : base(databaseService, configurationService, logger)
    {
        _validationService = validationService;
        _moveService = moveService;
        _fifoService = fifoService;
    }

    public async Task<MoveQuantityResponse> MoveQuantityWithFullValidationAsync(MoveQuantityRequest request, MoveQuantityOptions options)
    {
        // 1. Validate item
        var itemValidation = await _validationService.ValidateItemAsync(request.ItemCode, request.SiteReference);
        if (!itemValidation.IsValid)
        {
            return new MoveQuantityResponse
            {
                Success = false,
                ReturnCode = -1,
                ErrorMessage = $"Item validation failed: {itemValidation.ErrorMessage}"
            };
        }

        // 2. Validate lot
        var lotValidation = await _validationService.ValidateLotAsync(request.ItemCode, request.SourceLotNumber);
        if (!lotValidation.IsValid)
        {
            return new MoveQuantityResponse
            {
                Success = false,
                ReturnCode = -1,
                ErrorMessage = $"Lot validation failed: {lotValidation.ErrorMessage}"
            };
        }

        // 3. Validate source stock
        var warehouseCode = GetDefaultWarehouse(request.WarehouseCode);
        var sourceValidation = await _validationService.ValidateStockAvailabilityAsync(
            request.ItemCode, request.SourceLotNumber, request.SourceLocation, 
            request.Quantity, warehouseCode!).ConfigureAwait(false);
        if (!sourceValidation.IsAvailable)
        {
            return new MoveQuantityResponse
            {
                Success = false,
                ReturnCode = -1,
                ErrorMessage = sourceValidation.ErrorMessage
            };
        }

        // 4. Validate target location
        var targetValidation = await _validationService.ValidateLocationAsync(request.TargetLocation, request.SiteReference);
        if (!targetValidation.IsValid)
        {
            return new MoveQuantityResponse
            {
                Success = false,
                ReturnCode = -1,
                ErrorMessage = $"Target location validation failed: {targetValidation.ErrorMessage}"
            };
        }

        // 5. Check FIFO if requested
        if (options.ValidateFifo)
        {
            var fifoValidation = await _fifoService.ValidateFifoComplianceAsync(
                request.ItemCode, request.SourceLotNumber, warehouseCode!, request.SiteReference);
            if (!fifoValidation.IsCompliant && !string.IsNullOrEmpty(fifoValidation.WarningMessage))
            {
                Logger.LogWarning("FIFO violation detected: {WarningMessage}", fifoValidation.WarningMessage);
                // Continue with move but log warning - could also return warning response
            }
        }

        // 6. Perform move
        return await _moveService.MoveQuantityAsync(request);
    }
}

