using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Repositories;
using quantity_move_api.Services;
using quantity_move_api.Services.Query;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Repositories;

public class LocationRepositoryTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ILogger<LocationRepository>> _mockLogger;
    private readonly LocationRepository _repository;

    public LocationRepositoryTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        _mockLogger = new Mock<ILogger<LocationRepository>>();

        _repository = new LocationRepository(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetLocationAsync_WithValidLocation_ReturnsLocation()
    {
        // Arrange
        var locationCode = "LOC001";
        var locationResponse = new LocationValidationResponse
        {
            LocationCode = locationCode,
            LocationType = "Warehouse",
            Description = "Test Location",
            IsValid = true
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(locationResponse);

        // Act
        var result = await _repository.GetLocationAsync(locationCode);

        // Assert
        result.Should().NotBeNull();
        result!.LocationCode.Should().Be(locationCode);
        result.LocationType.Should().Be("Warehouse");
        result.Description.Should().Be("Test Location");
    }

    [Fact]
    public async Task GetLocationAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var locationCode = "LOC001";
        var siteReference = "SITE001";
        var locationResponse = new LocationValidationResponse
        {
            LocationCode = locationCode,
            IsValid = true
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(locationResponse);

        // Act
        var result = await _repository.GetLocationAsync(locationCode, siteReference);

        // Assert
        result.Should().NotBeNull();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetLocationAsync_WithNotFound_ReturnsNull()
    {
        // Arrange
        var locationCode = "LOC001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((LocationValidationResponse?)null);

        // Act
        var result = await _repository.GetLocationAsync(locationCode);

        // Assert
        result.Should().BeNull();
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
        var result = await _repository.GetLocationDetailsAsync(locationCode);

        // Assert
        result.Should().NotBeNull();
        result!.LocationCode.Should().Be(locationCode);
        result.WarehouseCode.Should().Be("WH001");
        result.SiteReference.Should().Be("SITE001");
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
            SiteReference = siteReference
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationDetailsResponse>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(locationDetails);

        // Act
        var result = await _repository.GetLocationDetailsAsync(locationCode, siteReference);

        // Assert
        result.Should().NotBeNull();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<LocationDetailsResponse>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetLocationDetailsAsync_WithNotFound_ReturnsNull()
    {
        // Arrange
        var locationCode = "LOC001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<LocationDetailsResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((LocationDetailsResponse?)null);

        // Act
        var result = await _repository.GetLocationDetailsAsync(locationCode);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task LocationExistsAsync_WithExistingLocation_ReturnsTrue()
    {
        // Arrange
        var locationCode = "LOC001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.LocationExistsAsync(locationCode);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task LocationExistsAsync_WithNonExistingLocation_ReturnsFalse()
    {
        // Arrange
        var locationCode = "LOC001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0);

        // Act
        var result = await _repository.LocationExistsAsync(locationCode);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task LocationExistsAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var locationCode = "LOC001";
        var siteReference = "SITE001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.LocationExistsAsync(locationCode, siteReference);

        // Assert
        result.Should().BeTrue();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<int>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }
}

