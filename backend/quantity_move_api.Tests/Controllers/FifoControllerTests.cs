using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Fifo;
using System.Security.Claims;

namespace quantity_move_api.Tests.Controllers;

public class FifoControllerTests
{
    private readonly Mock<IFifoService> _mockFifoService;
    private readonly Mock<ILogger<FifoController>> _mockLogger;
    private readonly IConfiguration _configuration;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly FifoController _controller;

    public FifoControllerTests()
    {
        _mockFifoService = new Mock<IFifoService>();
        _mockLogger = new Mock<ILogger<FifoController>>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Defaults:DefaultWarehouse", "MAIN" },
            { "Defaults:DefaultSite", "Default" }
        });
        _configuration = configBuilder.Build();
        
        _controller = new FifoController(
            _mockFifoService.Object,
            _mockLogger.Object,
            _configuration,
            _mockConfigurationService.Object);
        
        // Setup controller context for authorization
        _controller.ControllerContext = new ControllerContext
        {
            HttpContext = new DefaultHttpContext
            {
                User = new ClaimsPrincipal(new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, "testuser")
                }, "test"))
            }
        };
    }

    [Fact]
    public async Task GetOldestLot_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var siteReference = "SITE001";
        var expectedResponse = new FifoOldestLotResponse
        {
            ItemCode = itemCode,
            OldestLotNumber = "LOT001",
            LocationCode = warehouseCode,
            QuantityOnHand = 100
        };
        
        _mockFifoService.Setup(x => x.GetOldestLotAsync(itemCode, warehouseCode, siteReference))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetOldestLot(itemCode, warehouseCode, siteReference);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var apiResponse = okResult!.Value as ApiResponse<FifoOldestLotResponse>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetOldestLot_WithNullWarehouse_UsesDefault()
    {
        // Arrange
        var itemCode = "ITEM001";
        var defaultWarehouse = "DEFAULT_WH";
        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);
        
        var expectedResponse = new FifoOldestLotResponse
        {
            ItemCode = itemCode,
            OldestLotNumber = "LOT001"
        };
        
        _mockFifoService.Setup(x => x.GetOldestLotAsync(itemCode, defaultWarehouse, It.IsAny<string>()))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetOldestLot(itemCode, null, null);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockFifoService.Verify(x => x.GetOldestLotAsync(itemCode, defaultWarehouse, It.IsAny<string>()), Times.Once);
    }

    [Fact]
    public async Task GetOldestLot_WithException_ReturnsError()
    {
        // Arrange
        var itemCode = "ITEM001";
        _mockFifoService.Setup(x => x.GetOldestLotAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetOldestLot(itemCode, null, null);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        var apiResponse = objectResult.Value as ApiResponse<FifoOldestLotResponse>;
        apiResponse!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var request = new FifoValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            WarehouseCode = "WH001",
            SiteReference = "SITE001"
        };
        
        var expectedResponse = new FifoValidationResponse
        {
            IsCompliant = true,
            WarningMessage = null
        };
        
        _mockFifoService.Setup(x => x.ValidateFifoComplianceAsync(
                request.ItemCode, request.LotNumber, request.WarehouseCode, request.SiteReference))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Validate(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var apiResponse = okResult!.Value as ApiResponse<FifoValidationResponse>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Validate_WithInvalidModelState_ReturnsBadRequest()
    {
        // Arrange
        var request = new FifoValidationRequest
        {
            ItemCode = "", // Invalid
            LotNumber = "LOT001"
        };
        
        _controller.ModelState.AddModelError("ItemCode", "Item code is required");

        // Act
        var result = await _controller.Validate(request);

        // Assert
        result.Result.Should().BeOfType<BadRequestObjectResult>();
        var badRequest = result.Result as BadRequestObjectResult;
        var apiResponse = badRequest!.Value as ApiResponse<FifoValidationResponse>;
        apiResponse!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Validate_WithNullWarehouse_UsesDefault()
    {
        // Arrange
        var request = new FifoValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            WarehouseCode = null,
            SiteReference = null
        };
        
        var defaultWarehouse = "DEFAULT_WH";
        var defaultSite = "DEFAULT_SITE";
        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns(defaultSite);
        
        var expectedResponse = new FifoValidationResponse 
        { 
            IsCompliant = true,
            ItemCode = request.ItemCode,
            CurrentLotNumber = request.LotNumber
        };
        _mockFifoService.Setup(x => x.ValidateFifoComplianceAsync(
                request.ItemCode, request.LotNumber, defaultWarehouse, defaultSite))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Validate(request);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockFifoService.Verify(x => x.ValidateFifoComplianceAsync(
            request.ItemCode, request.LotNumber, defaultWarehouse, defaultSite), Times.Once);
    }

    [Fact]
    public async Task Validate_WithException_ReturnsError()
    {
        // Arrange
        var request = new FifoValidationRequest
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001"
        };
        
        _mockFifoService.Setup(x => x.ValidateFifoComplianceAsync(
                It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.Validate(request);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task GetSummary_WithValidRequest_ReturnsOk()
    {
        // Arrange
        var itemCode = "ITEM001";
        var warehouseCode = "WH001";
        var siteReference = "SITE001";
        var expectedResponse = new FifoSummaryResponse
        {
            ItemCode = itemCode,
            WarehouseCode = warehouseCode,
            Lots = new List<FifoSummaryLot>()
        };
        
        _mockFifoService.Setup(x => x.GetFifoSummaryAsync(itemCode, warehouseCode, siteReference))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetSummary(itemCode, warehouseCode, siteReference);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var apiResponse = okResult!.Value as ApiResponse<FifoSummaryResponse>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task GetSummary_WithNullParameters_UsesDefaults()
    {
        // Arrange
        var itemCode = "ITEM001";
        var defaultWarehouse = "DEFAULT_WH";
        var defaultSite = "DEFAULT_SITE";
        _mockConfigurationService.Setup(x => x.GetDefaultWarehouse())
            .Returns(defaultWarehouse);
        _mockConfigurationService.Setup(x => x.GetDefaultSite())
            .Returns(defaultSite);
        
        var expectedResponse = new FifoSummaryResponse 
        { 
            ItemCode = itemCode,
            WarehouseCode = defaultWarehouse,
            Lots = new List<FifoSummaryLot>()
        };
        _mockFifoService.Setup(x => x.GetFifoSummaryAsync(itemCode, defaultWarehouse, defaultSite))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.GetSummary(itemCode, null, null);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        _mockFifoService.Verify(x => x.GetFifoSummaryAsync(itemCode, defaultWarehouse, defaultSite), Times.Once);
    }

    [Fact]
    public async Task GetSummary_WithException_ReturnsError()
    {
        // Arrange
        var itemCode = "ITEM001";
        _mockFifoService.Setup(x => x.GetFifoSummaryAsync(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<string>()))
            .ThrowsAsync(new Exception("Service error"));

        // Act
        var result = await _controller.GetSummary(itemCode, null, null);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }
}

