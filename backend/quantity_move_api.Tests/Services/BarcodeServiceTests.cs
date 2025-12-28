using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Barcode;
using quantity_move_api.Services.Query;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Services;

public class BarcodeServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ILogger<BarcodeService>> _mockLogger;
    private readonly BarcodeService _service;

    public BarcodeServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        _mockLogger = new Mock<ILogger<BarcodeService>>();

        _service = new BarcodeService(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ParseBarcodeAsync_WithValidFormat_ReturnsParsedBarcode()
    {
        // Arrange
        var barcode = "ITEM001%LOT001";

        // Act
        var result = await _service.ParseBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.ItemCode.Should().Be("ITEM001");
        result.LotNumber.Should().Be("LOT001");
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task ParseBarcodeAsync_WithQuantity_ReturnsParsedBarcodeWithQuantity()
    {
        // Arrange
        var barcode = "ITEM001%LOT001%100.5";

        // Act
        var result = await _service.ParseBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.ItemCode.Should().Be("ITEM001");
        result.LotNumber.Should().Be("LOT001");
        result.Quantity.Should().Be(100.5m);
    }

    [Fact]
    public async Task ParseBarcodeAsync_WithInvalidQuantity_IgnoresQuantity()
    {
        // Arrange
        var barcode = "ITEM001%LOT001%INVALID";

        // Act
        var result = await _service.ParseBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeTrue();
        result.ItemCode.Should().Be("ITEM001");
        result.LotNumber.Should().Be("LOT001");
        result.Quantity.Should().BeNull();
    }

    [Fact]
    public async Task ParseBarcodeAsync_WithEmptyBarcode_ReturnsInvalid()
    {
        // Arrange
        var barcode = "";

        // Act
        var result = await _service.ParseBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Barcode is empty");
    }

    [Fact]
    public async Task ParseBarcodeAsync_WithWhitespaceBarcode_ReturnsInvalid()
    {
        // Arrange
        var barcode = "   ";

        // Act
        var result = await _service.ParseBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Barcode is empty");
    }

    [Fact]
    public async Task ParseBarcodeAsync_WithNullBarcode_ReturnsInvalid()
    {
        // Arrange
        string? barcode = null;

        // Act
        var result = await _service.ParseBarcodeAsync(barcode!);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Be("Barcode is empty");
    }

    [Fact]
    public async Task ParseBarcodeAsync_WithSinglePart_ReturnsInvalid()
    {
        // Arrange
        var barcode = "ITEM001";

        // Act
        var result = await _service.ParseBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.IsValid.Should().BeFalse();
        result.ErrorMessage.Should().Contain("Invalid barcode format");
    }

    [Fact]
    public async Task LookupBarcodeAsync_WithValidBarcodeAndFoundItem_ReturnsLookupResponse()
    {
        // Arrange
        var barcode = "ITEM001%LOT001";
        var lookupResponse = new BarcodeLookupResponse
        {
            ItemCode = "ITEM001",
            Description = "Test Item",
            Found = false // Will be set to true by service
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<BarcodeLookupResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(lookupResponse);

        // Act
        var result = await _service.LookupBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.Found.Should().BeTrue();
        result.ItemCode.Should().Be("ITEM001");
        result.LotNumber.Should().Be("LOT001");
        result.Description.Should().Be("Test Item");
        result.ErrorMessage.Should().BeNull();
    }

    [Fact]
    public async Task LookupBarcodeAsync_WithValidBarcodeAndNotFoundItem_ReturnsNotFound()
    {
        // Arrange
        var barcode = "ITEM001%LOT001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<BarcodeLookupResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((BarcodeLookupResponse?)null);

        // Act
        var result = await _service.LookupBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.Found.Should().BeFalse();
        result.ItemCode.Should().Be("ITEM001");
        result.LotNumber.Should().Be("LOT001");
        result.ErrorMessage.Should().Be("Item not found in database");
    }

    [Fact]
    public async Task LookupBarcodeAsync_WithInvalidBarcode_ReturnsParseError()
    {
        // Arrange
        var barcode = "INVALID";

        // Act
        var result = await _service.LookupBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.Found.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
        result.ErrorMessage.Should().Contain("Invalid barcode format");
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<BarcodeLookupResponse>(
            It.IsAny<string>(),
            It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task LookupBarcodeAsync_WithEmptyBarcode_ReturnsParseError()
    {
        // Arrange
        var barcode = "";

        // Act
        var result = await _service.LookupBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.Found.Should().BeFalse();
        result.ErrorMessage.Should().Be("Barcode is empty");
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<BarcodeLookupResponse>(
            It.IsAny<string>(),
            It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task LookupBarcodeAsync_WithValidParseButEmptyItemCode_ReturnsInvalid()
    {
        // Arrange
        var barcode = "%LOT001";

        // Act
        var result = await _service.LookupBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.Found.Should().BeFalse();
        result.ErrorMessage.Should().NotBeNull();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<BarcodeLookupResponse>(
            It.IsAny<string>(),
            It.IsAny<object>()), Times.Never);
    }

    [Fact]
    public async Task LookupBarcodeAsync_WithException_ThrowsException()
    {
        // Arrange
        var barcode = "ITEM001%LOT001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<BarcodeLookupResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.LookupBarcodeAsync(barcode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Fact]
    public async Task LookupBarcodeAsync_WithThreePartsButNoQuantity_StillParses()
    {
        // Arrange
        var barcode = "ITEM001%LOT001%";
        var lookupResponse = new BarcodeLookupResponse
        {
            ItemCode = "ITEM001",
            Description = "Test Item",
            Found = false
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<BarcodeLookupResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(lookupResponse);

        // Act
        var result = await _service.LookupBarcodeAsync(barcode);

        // Assert
        result.Should().NotBeNull();
        result.Found.Should().BeTrue();
        result.ItemCode.Should().Be("ITEM001");
        result.LotNumber.Should().Be("LOT001");
    }
}

