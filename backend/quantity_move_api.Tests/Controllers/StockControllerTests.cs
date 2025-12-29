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
    private readonly Mock<ILogger<StockController>> _mockLogger;
    private readonly IConfiguration _configuration;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly StockController _controller;

    public StockControllerTests()
    {
        _mockQueryService = new Mock<IStockQueryService>();
        _mockLogger = CreateMockLogger<StockController>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _configuration = CreateTestConfiguration();

        _controller = new StockController(
            _mockQueryService.Object,
            _mockLogger.Object,
            _configuration,
            _mockConfigurationService.Object);
    }

    [Fact]
    public async Task GetStockByBarcode_WithValidBarcode_ReturnsOk()
    {
        // Arrange
        var barcode = "ITEM001%LOT001";
        var expectedResponse = new LocationsResponse
        {
            Locations = new List<LocationItem>
            {
                new LocationItem { LocationCode = "LOC001", QuantityOnHand = 100.0m },
                new LocationItem { LocationCode = "LOC002", QuantityOnHand = 50.0m }
            }
        };

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        _mockQueryService.Setup(x => x.GetLocationsForItemLotAsync(
                "ITEM001", "LOT001", "WH001", false))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetStockByBarcode(barcode);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var response = okResult!.Value as ApiResponse<LocationsResponse>;
        response!.Success.Should().BeTrue();
        response.Data.Should().NotBeNull();
        response.Data!.Locations.Should().HaveCount(2);
    }

    [Fact]
    public async Task GetStockByBarcode_WithEmptyBarcode_ReturnsBadRequest()
    {
        // Arrange
        var barcode = "";

        // Act
        var result = await _controller.GetStockByBarcode(barcode);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        var response = badRequest!.Value as ApiResponse<LocationsResponse>;
        response!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task GetStockByBarcode_WithInvalidFormat_ReturnsBadRequest()
    {
        // Arrange
        var barcode = "ITEM001"; // Missing %

        // Act
        var result = await _controller.GetStockByBarcode(barcode);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        var response = badRequest!.Value as ApiResponse<LocationsResponse>;
        response!.Success.Should().BeFalse();
        response.Message.Should().Contain("Invalid barcode format");
    }

    [Fact]
    public async Task GetStockByBarcode_WithTooManyParts_ReturnsBadRequest()
    {
        // Arrange
        var barcode = "ITEM001%LOT001%EXTRA";

        // Act
        var result = await _controller.GetStockByBarcode(barcode);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetStockByBarcode_WithEmptyItemCode_ReturnsBadRequest()
    {
        // Arrange
        var barcode = "%LOT001";

        // Act
        var result = await _controller.GetStockByBarcode(barcode);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetStockByBarcode_WithEmptyLotNumber_ReturnsBadRequest()
    {
        // Arrange
        var barcode = "ITEM001%";

        // Act
        var result = await _controller.GetStockByBarcode(barcode);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
    }

    [Fact]
    public async Task GetStockByBarcode_WithWhitespace_TrimsCorrectly()
    {
        // Arrange
        var barcode = " ITEM001 % LOT001 ";
        var expectedResponse = new LocationsResponse
        {
            Locations = new List<LocationItem>()
        };

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        _mockQueryService.Setup(x => x.GetLocationsForItemLotAsync(
                "ITEM001", "LOT001", "WH001", false))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetStockByBarcode(barcode);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockQueryService.Verify(x => x.GetLocationsForItemLotAsync(
            "ITEM001", "LOT001", "WH001", false), Times.Once);
    }

    [Fact]
    public async Task GetStockByBarcode_WithException_ReturnsInternalServerError()
    {
        // Arrange
        var barcode = "ITEM001%LOT001";

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("WH001");

        _mockQueryService.Setup(x => x.GetLocationsForItemLotAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<bool>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.GetStockByBarcode(barcode);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetStockByBarcode_UsesDefaultWarehouse()
    {
        // Arrange
        var barcode = "ITEM001%LOT001";
        var expectedResponse = new LocationsResponse
        {
            Locations = new List<LocationItem>()
        };

        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns("DEFAULT_WH");

        _mockQueryService.Setup(x => x.GetLocationsForItemLotAsync(
                "ITEM001", "LOT001", "DEFAULT_WH", false))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetStockByBarcode(barcode);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockConfigurationService.Verify(x => x.GetDefaultWarehouse(), Times.Once);
    }
}
