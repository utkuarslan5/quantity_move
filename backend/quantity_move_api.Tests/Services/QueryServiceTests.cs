using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Services;
using quantity_move_api.Services.Query;

namespace quantity_move_api.Tests.Services;

public class QueryServiceTests
{
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<ILogger<QueryService>> _mockLogger;
    private readonly QueryService _service;

    public QueryServiceTests()
    {
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockLogger = new Mock<ILogger<QueryService>>();

        _service = new QueryService(_mockDatabaseService.Object, _mockLogger.Object);
    }

    [Fact]
    public async Task QueryAsync_DelegatesToDatabaseService()
    {
        // Arrange
        var sql = "SELECT * FROM Test";
        var parameters = new { Id = 1 };
        var expectedResult = new[] { "result1", "result2" };

        _mockDatabaseService.Setup(x => x.QueryAsync<string>(sql, parameters))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.QueryAsync<string>(sql, parameters);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _mockDatabaseService.Verify(x => x.QueryAsync<string>(sql, parameters), Times.Once);
    }

    [Fact]
    public async Task QueryAsync_WithNullParameters_DelegatesToDatabaseService()
    {
        // Arrange
        var sql = "SELECT * FROM Test";
        var expectedResult = new[] { "result1" };

        _mockDatabaseService.Setup(x => x.QueryAsync<string>(sql, null))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.QueryAsync<string>(sql, null);

        // Assert
        result.Should().BeEquivalentTo(expectedResult);
        _mockDatabaseService.Verify(x => x.QueryAsync<string>(sql, null), Times.Once);
    }

    [Fact]
    public async Task QueryFirstOrDefaultAsync_DelegatesToDatabaseService()
    {
        // Arrange
        var sql = "SELECT * FROM Test WHERE Id = @Id";
        var parameters = new { Id = 1 };
        var expectedResult = "result";

        _mockDatabaseService.Setup(x => x.QueryFirstOrDefaultAsync<string>(sql, parameters))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.QueryFirstOrDefaultAsync<string>(sql, parameters);

        // Assert
        result.Should().Be(expectedResult);
        _mockDatabaseService.Verify(x => x.QueryFirstOrDefaultAsync<string>(sql, parameters), Times.Once);
    }

    [Fact]
    public async Task QueryFirstOrDefaultAsync_WithNullParameters_DelegatesToDatabaseService()
    {
        // Arrange
        var sql = "SELECT * FROM Test";
        string? expectedResult = null;

        _mockDatabaseService.Setup(x => x.QueryFirstOrDefaultAsync<string>(sql, null))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.QueryFirstOrDefaultAsync<string>(sql, null);

        // Assert
        result.Should().BeNull();
        _mockDatabaseService.Verify(x => x.QueryFirstOrDefaultAsync<string>(sql, null), Times.Once);
    }

    [Fact]
    public async Task QuerySingleAsync_DelegatesToDatabaseService()
    {
        // Arrange
        var sql = "SELECT * FROM Test WHERE Id = @Id";
        var parameters = new { Id = 1 };
        var expectedResult = "result";

        _mockDatabaseService.Setup(x => x.QuerySingleAsync<string>(sql, parameters))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.QuerySingleAsync<string>(sql, parameters);

        // Assert
        result.Should().Be(expectedResult);
        _mockDatabaseService.Verify(x => x.QuerySingleAsync<string>(sql, parameters), Times.Once);
    }

    [Fact]
    public async Task QuerySingleAsync_WithNullParameters_DelegatesToDatabaseService()
    {
        // Arrange
        var sql = "SELECT * FROM Test";
        var expectedResult = "result";

        _mockDatabaseService.Setup(x => x.QuerySingleAsync<string>(sql, null))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.QuerySingleAsync<string>(sql, null);

        // Assert
        result.Should().Be(expectedResult);
        _mockDatabaseService.Verify(x => x.QuerySingleAsync<string>(sql, null), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_DelegatesToDatabaseService()
    {
        // Arrange
        var sql = "UPDATE Test SET Name = @Name";
        var parameters = new { Name = "Test" };
        var expectedResult = 1;

        _mockDatabaseService.Setup(x => x.ExecuteAsync(sql, parameters))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.ExecuteAsync(sql, parameters);

        // Assert
        result.Should().Be(expectedResult);
        _mockDatabaseService.Verify(x => x.ExecuteAsync(sql, parameters), Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_WithNullParameters_DelegatesToDatabaseService()
    {
        // Arrange
        var sql = "DELETE FROM Test";
        var expectedResult = 5;

        _mockDatabaseService.Setup(x => x.ExecuteAsync(sql, null))
            .ReturnsAsync(expectedResult);

        // Act
        var result = await _service.ExecuteAsync(sql, null);

        // Assert
        result.Should().Be(expectedResult);
        _mockDatabaseService.Verify(x => x.ExecuteAsync(sql, null), Times.Once);
    }
}

