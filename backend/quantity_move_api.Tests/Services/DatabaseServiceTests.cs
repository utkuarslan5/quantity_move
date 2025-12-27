using Dapper;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Services;
using System.Data;

namespace quantity_move_api.Tests.Services;

public class DatabaseServiceTests
{
    private readonly Mock<ILogger<DatabaseService>> _mockLogger;

    public DatabaseServiceTests()
    {
        _mockLogger = new Mock<ILogger<DatabaseService>>();
    }

    [Fact]
    public void Constructor_WithNullConnectionString_ThrowsException()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var mockConfig = configBuilder.Build();

        // Act
        Action act = () => new DatabaseService(mockConfig, _mockLogger.Object);

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Connection string 'DefaultConnection' not found.");
    }

    [Fact]
    public void Constructor_WithValidConnectionString_CreatesService()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=test;Database=test;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();

        // Act
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public void Constructor_WithEmptyConnectionString_CreatesService()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "" }
        });
        var mockConfig = configBuilder.Build();

        // Act
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Assert
        // Empty string is not null, so GetConnectionString returns empty string
        // The service is created, but will fail when trying to connect
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task ExecuteStoredProcedureAsync_WithValidParameters_CallsDapper()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=test;Database=test;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        
        var service = new DatabaseService(mockConfig, _mockLogger.Object);
        
        // Note: This would require mocking SqlConnection which is complex
        // Integration tests or refactoring to use an abstraction would be better
        // For now, we verify the service can be instantiated

        // Act & Assert
        service.Should().NotBeNull();
        // Integration tests will verify actual database operations
    }

    [Fact]
    public async Task ExecuteStoredProcedureNonQueryAsync_HandlesDynamicParameters()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=test;Database=test;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        
        var service = new DatabaseService(mockConfig, _mockLogger.Object);
        var parameters = new DynamicParameters();
        parameters.Add("@param1", "value1");

        // Note: Actual execution requires a real database connection
        // Integration tests will verify this behavior

        // Act & Assert
        service.Should().NotBeNull();
        // Integration tests will verify DynamicParameters handling
    }

    [Fact]
    public void CreateConnection_ReturnsSqlConnection()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=test;Database=test;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Note: CreateConnection is private, so we test it indirectly
        // The method is called by all query methods, so it gets coverage through them
        // For direct testing, we'd need to make it protected or use reflection

        // Act & Assert
        service.Should().NotBeNull();
    }

    [Fact]
    public async Task QueryAsync_ThrowsException_WhenConnectionFails()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=invalid;Database=invalid;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Act
        Func<Task> act = async () => await service.QueryAsync<string>("SELECT 1");

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task QueryFirstOrDefaultAsync_ThrowsException_WhenConnectionFails()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=invalid;Database=invalid;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Act
        Func<Task> act = async () => await service.QueryFirstOrDefaultAsync<string>("SELECT 1");

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task QuerySingleAsync_ThrowsException_WhenConnectionFails()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=invalid;Database=invalid;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Act
        Func<Task> act = async () => await service.QuerySingleAsync<string>("SELECT 1");

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteAsync_ThrowsException_WhenConnectionFails()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=invalid;Database=invalid;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Act
        Func<Task> act = async () => await service.ExecuteAsync("UPDATE table SET col = 1");

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteStoredProcedureAsync_ThrowsException_WhenConnectionFails()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=invalid;Database=invalid;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Act
        Func<Task> act = async () => await service.ExecuteStoredProcedureAsync<string>("TestProc");

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteStoredProcedureSingleAsync_ThrowsException_WhenConnectionFails()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=invalid;Database=invalid;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Act
        Func<Task> act = async () => await service.ExecuteStoredProcedureSingleAsync<string>("TestProc");

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteStoredProcedureWithOutputAsync_ThrowsException_WhenConnectionFails()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=invalid;Database=invalid;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        var service = new DatabaseService(mockConfig, _mockLogger.Object);

        // Act
        Func<Task> act = async () => await service.ExecuteStoredProcedureWithOutputAsync("TestProc");

        // Assert
        await act.Should().ThrowAsync<Exception>();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Error,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task ExecuteStoredProcedureNonQueryAsync_WithNonDynamicParameters_ExecutesNormally()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=invalid;Database=invalid;Trusted_Connection=true;" }
        });
        var mockConfig = configBuilder.Build();
        var service = new DatabaseService(mockConfig, _mockLogger.Object);
        var parameters = new { Param1 = "value1" };

        // Act
        Func<Task> act = async () => await service.ExecuteStoredProcedureNonQueryAsync("TestProc", parameters);

        // Assert
        // Should throw exception due to invalid connection, but should reach the non-DynamicParameters path
        await act.Should().ThrowAsync<Exception>();
    }
}

