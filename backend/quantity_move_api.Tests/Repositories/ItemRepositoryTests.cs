using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Repositories;
using quantity_move_api.Services;
using quantity_move_api.Services.Query;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Repositories;

public class ItemRepositoryTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly Mock<ILogger<ItemRepository>> _mockLogger;
    private readonly ItemRepository _repository;

    public ItemRepositoryTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        _mockLogger = new Mock<ILogger<ItemRepository>>();

        _repository = new ItemRepository(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task GetItemAsync_WithValidItem_ReturnsItem()
    {
        // Arrange
        var itemCode = "ITEM001";
        var itemResponse = new ItemValidationResponse
        {
            ItemCode = itemCode,
            IsLotTracked = true,
            Description = "Test Item"
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        // Act
        var result = await _repository.GetItemAsync(itemCode);

        // Assert
        result.Should().NotBeNull();
        result!.ItemCode.Should().Be(itemCode);
        result.IsLotTracked.Should().BeTrue();
        result.Description.Should().Be("Test Item");
    }

    [Fact]
    public async Task GetItemAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var itemCode = "ITEM001";
        var siteReference = "SITE001";
        var itemResponse = new ItemValidationResponse
        {
            ItemCode = itemCode,
            IsLotTracked = true
        };

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(itemResponse);

        // Act
        var result = await _repository.GetItemAsync(itemCode, siteReference);

        // Assert
        result.Should().NotBeNull();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task GetItemAsync_WithNotFound_ReturnsNull()
    {
        // Arrange
        var itemCode = "ITEM001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<ItemValidationResponse>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((ItemValidationResponse?)null);

        // Act
        var result = await _repository.GetItemAsync(itemCode);

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task IsLotTrackedAsync_WithLotTrackedItem_ReturnsTrue()
    {
        // Arrange
        var itemCode = "ITEM001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<bool?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(true);

        // Act
        var result = await _repository.IsLotTrackedAsync(itemCode);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task IsLotTrackedAsync_WithNonLotTrackedItem_ReturnsFalse()
    {
        // Arrange
        var itemCode = "ITEM001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<bool?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(false);

        // Act
        var result = await _repository.IsLotTrackedAsync(itemCode);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsLotTrackedAsync_WithNullResult_ReturnsFalse()
    {
        // Arrange
        var itemCode = "ITEM001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<bool?>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((bool?)null);

        // Act
        var result = await _repository.IsLotTrackedAsync(itemCode);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task IsLotTrackedAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var itemCode = "ITEM001";
        var siteReference = "SITE001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<bool?>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(true);

        // Act
        var result = await _repository.IsLotTrackedAsync(itemCode, siteReference);

        // Assert
        result.Should().BeTrue();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<bool?>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }

    [Fact]
    public async Task ItemExistsAsync_WithExistingItem_ReturnsTrue()
    {
        // Arrange
        var itemCode = "ITEM001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.ItemExistsAsync(itemCode);

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public async Task ItemExistsAsync_WithNonExistingItem_ReturnsFalse()
    {
        // Arrange
        var itemCode = "ITEM001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(0);

        // Act
        var result = await _repository.ItemExistsAsync(itemCode);

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public async Task ItemExistsAsync_WithSiteReference_IncludesSiteInQuery()
    {
        // Arrange
        var itemCode = "ITEM001";
        var siteReference = "SITE001";

        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<int>(
                It.Is<string>(s => s.Contains("SiteReference")),
                It.IsAny<object>()))
            .ReturnsAsync(1);

        // Act
        var result = await _repository.ItemExistsAsync(itemCode, siteReference);

        // Assert
        result.Should().BeTrue();
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<int>(
            It.Is<string>(s => s.Contains("SiteReference")),
            It.IsAny<object>()), Times.Once);
    }
}

