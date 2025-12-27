using FluentAssertions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Controllers;

public class StockControllerTests
{
    private readonly Mock<IStockService> _mockStockService;
    private readonly Mock<ILogger<StockController>> _mockLogger;
    private readonly Mock<IConfiguration> _mockConfiguration;
    private readonly StockController _controller;

    public StockControllerTests()
    {
        _mockStockService = new Mock<IStockService>();
        _mockLogger = new Mock<ILogger<StockController>>();
        _mockConfiguration = new Mock<IConfiguration>();
        _controller = new StockController(_mockStockService.Object, _mockLogger.Object, _mockConfiguration.Object);
    }

    [Fact]
    public async Task Lookup_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = TestHelpers.CreateStockLookupRequest();
        var response = TestHelpers.CreateStockLookupResponse();
        _mockStockService.Setup(x => x.GetStockAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Lookup(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        okResult!.Value.Should().BeOfType<ApiResponse<StockLookupResponse>>();
        
        var apiResponse = okResult.Value as ApiResponse<StockLookupResponse>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().NotBeNull();
        apiResponse.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Lookup_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var request = TestHelpers.CreateStockLookupRequest();
        _controller.ModelState.AddModelError("ItemCode", "ItemCode is invalid");

        // Act
        var result = await _controller.Lookup(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequestResult = result.Result as BadRequestObjectResult;
        badRequestResult!.Value.Should().BeOfType<ApiResponse<StockLookupResponse>>();
        
        var response = badRequestResult.Value as ApiResponse<StockLookupResponse>;
        response!.Success.Should().BeFalse();
        response.Errors.Should().NotBeEmpty();
    }

    [Fact]
    public async Task Lookup_WhenServiceThrowsException_ReturnsInternalServerError()
    {
        // Arrange
        var request = TestHelpers.CreateStockLookupRequest();
        _mockStockService.Setup(x => x.GetStockAsync(request))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        var result = await _controller.Lookup(request);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        objectResult.Value.Should().BeOfType<ApiResponse<StockLookupResponse>>();
        
        var response = objectResult.Value as ApiResponse<StockLookupResponse>;
        response!.Success.Should().BeFalse();
        response.Message.Should().Contain("error occurred");
    }

    [Fact]
    public async Task LookupGet_WithQueryParameters_CreatesRequestAndCallsLookup()
    {
        // Arrange
        var itemCode = "ITEM001";
        var location = "LOC001";
        var warehouse = "WH001";
        var site = "SITE001";
        var lot = "LOT001";
        
        var expectedRequest = new StockLookupRequest
        {
            ItemCode = itemCode,
            Location = location,
            Warehouse = warehouse,
            Site = site,
            Lot = lot
        };
        
        var response = TestHelpers.CreateStockLookupResponse();
        _mockStockService.Setup(x => x.GetStockAsync(It.Is<StockLookupRequest>(r => 
            r.ItemCode == itemCode && 
            r.Location == location &&
            r.Warehouse == warehouse &&
            r.Site == site &&
            r.Lot == lot)))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.LookupGet(itemCode, location, warehouse, site, lot);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var apiResponse = okResult!.Value as ApiResponse<StockLookupResponse>;
        
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data!.Items.Should().NotBeEmpty();
    }

    [Fact]
    public async Task LookupGet_WithNullParameters_StillWorks()
    {
        // Arrange
        var response = new StockLookupResponse { Items = new List<StockItem>() };
        _mockStockService.Setup(x => x.GetStockAsync(It.IsAny<StockLookupRequest>()))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.LookupGet(null, null, null, null, null);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
    }

    [Fact]
    public async Task Lookup_ReturnsStockItemsWithCorrectProperties()
    {
        // Arrange
        var request = TestHelpers.CreateStockLookupRequest();
        var response = new StockLookupResponse
        {
            Items = new List<StockItem>
            {
                new StockItem
                {
                    ItemCode = "ITEM001",
                    Location = "LOC001",
                    Quantity = 100.0m,
                    ExpiryDate = DateTime.Now.AddDays(30),
                    ProductionDate = DateTime.Now.AddDays(-10)
                }
            }
        };
        _mockStockService.Setup(x => x.GetStockAsync(request))
            .ReturnsAsync(response);

        // Act
        var result = await _controller.Lookup(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var apiResponse = okResult!.Value as ApiResponse<StockLookupResponse>;
        
        apiResponse!.Data!.Items.Should().HaveCount(1);
        apiResponse.Data.Items[0].ItemCode.Should().Be("ITEM001");
        apiResponse.Data.Items[0].Quantity.Should().Be(100.0m);
    }
}

