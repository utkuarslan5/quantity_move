using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Stock;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Controllers;

public class StockControllerTests : ControllerTestBase
{
    private readonly Mock<IStockQueryService> _mockQueryService;
    private readonly Mock<IStockValidationService> _mockValidationService;
    private readonly Mock<IStockLocationService> _mockLocationService;
    private readonly Mock<ILogger<StockController>> _mockLogger;
    private readonly IConfiguration _configuration;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly StockController _controller;

    public StockControllerTests()
    {
        _mockQueryService = new Mock<IStockQueryService>();
        _mockValidationService = new Mock<IStockValidationService>();
        _mockLocationService = new Mock<IStockLocationService>();
        _mockLogger = CreateMockLogger<StockController>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _configuration = CreateTestConfiguration();

        _controller = new StockController(
            _mockQueryService.Object,
            _mockValidationService.Object,
            _mockLocationService.Object,
            _mockLogger.Object,
            _configuration,
            _mockConfigurationService.Object);
    }

    [Fact]
    public async Task GetQuantity_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new GetQuantityRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            LocationCode = "LOC001",
            WarehouseCode = "WH001"
        };
        var expectedQuantity = 100.5m;

        _mockQueryService.Setup(x => x.GetQuantityAtLocationAsync(
                request.ItemCode, request.LotNumber, request.LocationCode, request.WarehouseCode))
            .ReturnsAsync(expectedQuantity);

        // Act
        var result = await _controller.GetQuantity(request);

        // Assert
        result.Should().BeOfType<ActionResult<ApiResponse<decimal>>>();
        var okResult = result.Result as OkObjectResult;
        okResult.Should().NotBeNull();
        var response = okResult!.Value as ApiResponse<decimal>;
        response!.Success.Should().BeTrue();
        response.Data.Should().Be(expectedQuantity);
    }

    [Fact]
    public async Task GetQuantity_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var request = new GetQuantityRequest();
        _controller.ModelState.AddModelError("ItemCode", "ItemCode is required");

        // Act
        var result = await _controller.GetQuantity(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetQuantity_WithException_ReturnsInternalServerError()
    {
        // Arrange
        var request = new GetQuantityRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            LocationCode = "LOC001",
            WarehouseCode = "WH001"
        };

        _mockQueryService.Setup(x => x.GetQuantityAtLocationAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetQuantity(request);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetLocations_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new GetLocationsRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            WarehouseCode = "WH001",
            IncludeZeroQuantity = false
        };
        var expectedResponse = new LocationsResponse
        {
            Locations = new List<LocationItem>
            {
                new LocationItem { LocationCode = "LOC001", QuantityOnHand = 100.0m }
            }
        };

        _mockQueryService.Setup(x => x.GetLocationsForItemLotAsync(
                request.ItemCode, request.LotNumber, request.WarehouseCode, request.IncludeZeroQuantity))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetLocations(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetLocationsWithStock_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new GetLocationsRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            WarehouseCode = "WH001"
        };
        var expectedResponse = new LocationsWithStockResponse
        {
            Locations = new List<LocationWithStockItem>
            {
                new LocationWithStockItem { LocationCode = "LOC001", QuantityOnHand = 100.0m }
            }
        };

        _mockLocationService.Setup(x => x.GetLocationsWithStockAsync(
                request.ItemCode, request.LotNumber, request.WarehouseCode))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetLocationsWithStock(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetCurrentLocation_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new GetCurrentLocationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            WarehouseCode = "WH001"
        };
        var expectedResponse = new CurrentLocationResponse
        {
            ItemCode = request.ItemCode,
            LotNumber = request.LotNumber,
            Locations = new List<CurrentLocationItem>()
        };

        _mockQueryService.Setup(x => x.GetCurrentLocationAsync(
                request.ItemCode, request.LotNumber, request.WarehouseCode))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetCurrentLocation(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task GetSummary_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var expectedResponse = new StockSummaryResponse
        {
            ItemCode = itemCode,
            WarehouseCode = warehouseCode,
            TotalQuantity = 100.0m,
            LocationCount = 1,
            Locations = new List<StockSummaryLocation>()
        };

        _mockQueryService.Setup(x => x.GetStockSummaryAsync(itemCode, warehouseCode))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetSummary(itemCode, warehouseCode);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ValidateItem_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new ValidateItemRequest
        {
            ItemCode = "ITEM001",
            SiteReference = "SITE001"
        };
        var expectedResponse = new ItemValidationResponse
        {
            ItemCode = request.ItemCode,
            IsLotTracked = true,
            IsValid = true
        };

        _mockValidationService.Setup(x => x.ValidateItemAsync(request.ItemCode, request.SiteReference))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateItem(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ValidateLot_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new ValidateLotRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001"
        };
        var expectedResponse = new LotValidationResponse
        {
            ItemCode = request.ItemCode,
            LotNumber = request.LotNumber,
            IsValid = true
        };

        _mockValidationService.Setup(x => x.ValidateLotAsync(request.ItemCode, request.LotNumber))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateLot(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ValidateLocation_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new ValidateLocationRequest
        {
            LocationCode = "LOC001",
            SiteReference = "SITE001"
        };
        var expectedResponse = new LocationValidationResponse
        {
            LocationCode = request.LocationCode,
            IsValid = true
        };

        _mockValidationService.Setup(x => x.ValidateLocationAsync(request.LocationCode, request.SiteReference))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateLocation(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ValidateAvailability_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new ValidateStockRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            LocationCode = "LOC001",
            RequiredQuantity = 50.0m,
            WarehouseCode = "WH001"
        };
        var expectedResponse = new StockAvailabilityResponse
        {
            IsAvailable = true,
            ItemCode = request.ItemCode,
            LotNumber = request.LotNumber,
            LocationCode = request.LocationCode
        };

        _mockValidationService.Setup(x => x.ValidateStockAvailabilityAsync(
                request.ItemCode, request.LotNumber, request.LocationCode,
                request.RequiredQuantity, request.WarehouseCode))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.ValidateAvailability(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ValidateForMove_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new StockMoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = "WH001",
            SiteReference = "SITE001"
        };
        var expectedResponse = new CombinedValidationResponse
        {
            IsValid = true
        };

        _mockValidationService.Setup(x => x.ValidateStockForMoveAsync(request))
            .ReturnsAsync(expectedResponse);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns("SITE001");

        // Act
        var result = await _controller.ValidateForMove(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task ValidateForMove_WithNullWarehouse_UsesDefault()
    {
        // Arrange
        var request = new StockMoveValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            SourceLocation = "LOC001",
            TargetLocation = "LOC002",
            Quantity = 50.0m,
            WarehouseCode = null,
            SiteReference = null
        };
        var expectedResponse = new CombinedValidationResponse
        {
            IsValid = true
        };

        _mockValidationService.Setup(x => x.ValidateStockForMoveAsync(It.IsAny<StockMoveValidationRequest>()))
            .ReturnsAsync(expectedResponse);

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("DEFAULT_WH");
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns("DEFAULT_SITE");

        // Act
        var result = await _controller.ValidateForMove(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockConfigurationService.Verify(x => x.GetDefaultWarehouse(), Times.Once);
        _mockConfigurationService.Verify(x => x.GetDefaultSite(), Times.Once);
    }

    [Fact]
    public async Task GetQuantity_WithNullWarehouse_UsesDefault()
    {
        // Arrange
        var request = new GetQuantityRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            LocationCode = "LOC001",
            WarehouseCode = null
        };

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("DEFAULT_WH");

        _mockQueryService.Setup(x => x.GetQuantityAtLocationAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), "DEFAULT_WH"))
            .ReturnsAsync(100.0m);

        // Act
        var result = await _controller.GetQuantity(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockConfigurationService.Verify(x => x.GetDefaultWarehouse(), Times.Once);
    }
}

