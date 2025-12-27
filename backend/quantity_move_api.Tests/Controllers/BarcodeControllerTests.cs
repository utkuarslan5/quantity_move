using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Controllers;
using quantity_move_api.Models;
using quantity_move_api.Services.Barcode;
using System.Security.Claims;

namespace quantity_move_api.Tests.Controllers;

public class BarcodeControllerTests
{
    private readonly Mock<IBarcodeService> _mockBarcodeService;
    private readonly Mock<ILogger<BarcodeController>> _mockLogger;
    private readonly IConfiguration _configuration;
    private readonly BarcodeController _controller;

    public BarcodeControllerTests()
    {
        _mockBarcodeService = new Mock<IBarcodeService>();
        _mockLogger = new Mock<ILogger<BarcodeController>>();
        
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        _configuration = configBuilder.Build();
        
        _controller = new BarcodeController(
            _mockBarcodeService.Object,
            _mockLogger.Object,
            _configuration);
        
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
    public async Task Parse_WithValidBarcode_ReturnsOk()
    {
        // Arrange
        var barcode = "ITEM001%LOT001%100";
        var expectedResponse = new BarcodeParseResponse
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            Quantity = 100,
            IsValid = true
        };
        
        _mockBarcodeService.Setup(x => x.ParseBarcodeAsync(barcode))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Parse(barcode);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var apiResponse = okResult!.Value as ApiResponse<BarcodeParseResponse>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Parse_WithException_ReturnsError()
    {
        // Arrange
        var barcode = "INVALID";
        _mockBarcodeService.Setup(x => x.ParseBarcodeAsync(barcode))
            .ThrowsAsync(new Exception("Invalid barcode format"));

        // Act
        var result = await _controller.Parse(barcode);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        var apiResponse = objectResult.Value as ApiResponse<BarcodeParseResponse>;
        apiResponse!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Parse_WithNullBarcode_ThrowsException()
    {
        // Arrange
        string? barcode = null;
        _mockBarcodeService.Setup(x => x.ParseBarcodeAsync(It.IsAny<string>()))
            .ThrowsAsync(new ArgumentNullException(nameof(barcode)));

        // Act
        var result = await _controller.Parse(barcode!);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Parse_WithEmptyBarcode_ReturnsError()
    {
        // Arrange
        var barcode = "";
        _mockBarcodeService.Setup(x => x.ParseBarcodeAsync(barcode))
            .ThrowsAsync(new ArgumentException("Barcode cannot be empty"));

        // Act
        var result = await _controller.Parse(barcode);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Lookup_WithValidBarcode_ReturnsOk()
    {
        // Arrange
        var barcode = "ITEM001%LOT001%100";
        var expectedResponse = new BarcodeLookupResponse
        {
            ItemCode = "ITEM001",
            LotNumber = "LOT001",
            Description = "Test Item",
            Found = true
        };
        
        _mockBarcodeService.Setup(x => x.LookupBarcodeAsync(barcode))
            .ReturnsAsync(expectedResponse);

        // Act
        var result = await _controller.Lookup(barcode);

        // Assert
        result.Result.Should().BeOfType<OkObjectResult>();
        var okResult = result.Result as OkObjectResult;
        var apiResponse = okResult!.Value as ApiResponse<BarcodeLookupResponse>;
        apiResponse!.Success.Should().BeTrue();
        apiResponse.Data.Should().BeEquivalentTo(expectedResponse);
    }

    [Fact]
    public async Task Lookup_WithException_ReturnsError()
    {
        // Arrange
        var barcode = "NOTFOUND";
        _mockBarcodeService.Setup(x => x.LookupBarcodeAsync(barcode))
            .ThrowsAsync(new KeyNotFoundException("Barcode not found"));

        // Act
        var result = await _controller.Lookup(barcode);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
        var apiResponse = objectResult.Value as ApiResponse<BarcodeLookupResponse>;
        apiResponse!.Success.Should().BeFalse();
    }

    [Fact]
    public async Task Lookup_WithNullBarcode_ThrowsException()
    {
        // Arrange
        string? barcode = null;
        _mockBarcodeService.Setup(x => x.LookupBarcodeAsync(It.IsAny<string>()))
            .ThrowsAsync(new ArgumentNullException(nameof(barcode)));

        // Act
        var result = await _controller.Lookup(barcode!);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }

    [Fact]
    public async Task Lookup_WithEmptyBarcode_ReturnsError()
    {
        // Arrange
        var barcode = "";
        _mockBarcodeService.Setup(x => x.LookupBarcodeAsync(barcode))
            .ThrowsAsync(new ArgumentException("Barcode cannot be empty"));

        // Act
        var result = await _controller.Lookup(barcode);

        // Assert
        result.Result.Should().BeOfType<ObjectResult>();
        var objectResult = result.Result as ObjectResult;
        objectResult!.StatusCode.Should().Be(500);
    }
}

