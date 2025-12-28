using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Query;
using quantity_move_api.Services.Stock;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Services;

public class StockLocationServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ILogger<StockLocationService>> _mockLogger;
    private readonly StockLocationService _service;

    public StockLocationServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        _mockLogger = new Mock<ILogger<StockLocationService>>();

        _service = new StockLocationService(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetLocationsWithStockAsync_WithValidData_ReturnsLocations()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var locations = new List<LocationWithStockItem>
        {
            new LocationWithStockItem { LocationCode = "LOC001", QuantityOnHand = 100.0m, LocationDisplay = "LOC001-100.00" },
            new LocationWithStockItem { LocationCode = "LOC002", QuantityOnHand = 50.0m, LocationDisplay = "LOC002-50.00" }
        };

        _mockQueryService.Setup(x => x.QueryAsync<LocationWithStockItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locations);

        // Act
        var result = await _service.GetLocationsWithStockAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.Locations.Should().HaveCount(2);
        result.Locations.Should().Contain(l => l.LocationCode == "LOC001");
        result.Locations.Should().Contain(l => l.LocationCode == "LOC002");
    }

    [Fact]
    public async Task GetLocationsWithStockAsync_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var emptyLocations = new List<LocationWithStockItem>();

        _mockQueryService.Setup(x => x.QueryAsync<LocationWithStockItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(emptyLocations);

        // Act
        var result = await _service.GetLocationsWithStockAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.Locations.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLocationsWithStockAsync_WithException_ThrowsException()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";

        _mockQueryService.Setup(x => x.QueryAsync<LocationWithStockItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.GetLocationsWithStockAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }

    [Fact]
    public async Task GetLocationsWithoutStockAsync_WithValidData_ReturnsLocations()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var locations = new List<LocationItem>
        {
            new LocationItem { LocationCode = "LOC001", QuantityOnHand = 0.0m, LocationDisplay = "LOC001-0.00" },
            new LocationItem { LocationCode = "LOC002", QuantityOnHand = 0.0m, LocationDisplay = "LOC002-0.00" }
        };

        _mockQueryService.Setup(x => x.QueryAsync<LocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locations);

        // Act
        var result = await _service.GetLocationsWithoutStockAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.Locations.Should().HaveCount(2);
        result.Locations.Should().AllSatisfy(l => l.QuantityOnHand.Should().Be(0));
    }

    [Fact]
    public async Task GetLocationsWithoutStockAsync_WithEmptyResult_ReturnsEmptyList()
    {
        // Arrange
        var itemCode = "ITEM001";
        var lotNumber = "LOT001";
        var warehouseCode = "WH001";
        var emptyLocations = new List<LocationItem>();

        _mockQueryService.Setup(x => x.QueryAsync<LocationItem>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(emptyLocations);

        // Act
        var result = await _service.GetLocationsWithoutStockAsync(itemCode, lotNumber, warehouseCode);

        // Assert
        result.Should().NotBeNull();
        result.Locations.Should().BeEmpty();
    }

    [Fact]
    public async Task GetLocationDetailsAsync_WithValidLocation_ReturnsDetails()
    {
        // Arrange
        var locationCode = "LOC001";
        var locationDetails = new LocationDetailsResponse
        {
            LocationCode = locationCode,
            LocationType = "Warehouse",
            Description = "Test Location",
            WarehouseCode = "WH001",
            SiteReference = "SITE001"
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationDetailsResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locationDetails);

        // Act
        var result = await _service.GetLocationDetailsAsync(locationCode);

        // Assert
        result.Should().NotBeNull();
        result.LocationCode.Should().Be(locationCode);
        result.LocationType.Should().Be("Warehouse");
        result.Description.Should().Be("Test Location");
    }

    [Fact]
    public async Task GetLocationDetailsAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var locationCode = "LOC001";
        var siteReference = "SITE001";
        var locationDetails = new LocationDetailsResponse
        {
            LocationCode = locationCode,
            LocationType = "Warehouse",
            Description = "Test Location",
            WarehouseCode = "WH001",
            SiteReference = siteReference
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationDetailsResponse>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(locationDetails);

        // Act
        var result = await _service.GetLocationDetailsAsync(locationCode, siteReference);

        // Assert
        result.Should().NotBeNull();
        result.SiteReference.Should().Be(siteReference);
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<LocationDetailsResponse>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetLocationDetailsAsync_WithLocationNotFound_ThrowsException()
    {
        // Arrange
        var locationCode = "LOC001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationDetailsResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((LocationDetailsResponse?)null);

        // Act
        Func<Task> act = async () => await _service.GetLocationDetailsAsync(locationCode);

        // Assert
        await act.Should().ThrowAsync<InvalidOperationException>()
            .WithMessage($"Location {locationCode} not found");
    }

    [Fact]
    public async Task GetLocationDetailsAsync_WithException_ThrowsException()
    {
        // Arrange
        var locationCode = "LOC001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationDetailsResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Database error"));

        // Act
        Func<Task> act = async () => await _service.GetLocationDetailsAsync(locationCode);

        // Assert
        await act.Should().ThrowAsync<Exception>().WithMessage("Database error");
    }
}

