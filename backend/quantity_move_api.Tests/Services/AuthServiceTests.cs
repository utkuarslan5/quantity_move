using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Services.Query;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Services;

public class AuthServiceTests
{
    private readonly IConfiguration _mockConfiguration;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly Mock<IConfigurationService> _mockConfigurationService;
    private readonly Mock<IQueryService> _mockQueryService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockDatabaseService = new Mock<IDatabaseService>();
        _mockConfigurationService = new Mock<IConfigurationService>();
        _mockQueryService = new Mock<IQueryService>();
        
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=test;Database=test;Trusted_Connection=true;" }
        });
        _mockConfiguration = configBuilder.Build();
        
        _authService = new AuthService(
            _mockDatabaseService.Object,
            _mockConfigurationService.Object,
            _mockQueryService.Object,
            _mockLogger.Object);
    }

    [Fact]
    public async Task ValidateUserAsync_WithTRM_EDIUSERTable_ReturnsUser()
    {
        // Arrange
        var username = "testuser";
        var password = "testpass";
        var expectedUser = TestHelpers.CreateTestUser(username: username, password: password);
        expectedUser.Warehouse = "MAIN";
        
        // The actual query uses TableNames.UserMaster which is "TRM_EDIUSER"
        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<User>(
                It.Is<string>(s => s.Contains("TRM_EDIUSER")),
                It.IsAny<object>()))
            .ReturnsAsync(expectedUser);
        
        // Setup employee_mst query (optional validation)
        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<dynamic>(
                It.Is<string>(s => s.Contains("employee_mst")),
                It.IsAny<object>()))
            .ReturnsAsync(new { emp_num = username, site_Ref = "faz", full_name = "Test User" });

        // Act
        var result = await _authService.ValidateUserAsync(username, password);

        // Assert
        result.Should().NotBeNull();
        result!.Username.Should().Be(username);
        result.Warehouse.Should().Be("MAIN");
        _mockQueryService.Verify(x => x.QueryFirstOrDefaultAsync<User>(
            It.Is<string>(s => s.Contains("TRM_EDIUSER")),
            It.IsAny<object>()), Times.Once);
    }


    [Fact]
    public async Task ValidateUserAsync_WithAllQueriesFailing_ReturnsNull()
    {
        // Arrange
        var username = "testuser";
        var password = "testpass";
        
        // All queries fail
        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<User>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Table not found"));

        // Act
        var result = await _authService.ValidateUserAsync(username, password);

        // Assert
        result.Should().BeNull();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Never);
    }

    [Fact]
    public async Task ValidateUserAsync_WithAllQueriesReturningNull_ReturnsNull()
    {
        // Arrange
        var username = "testuser";
        var password = "testpass";
        
        // All queries return null
        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<User>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync((User?)null);

        // Act
        var result = await _authService.ValidateUserAsync(username, password);

        // Assert
        result.Should().BeNull();
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Warning,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }

    [Fact]
    public async Task ValidateUserAsync_WithExceptionInOuterCatch_ReturnsNull()
    {
        // Arrange
        var username = "testuser";
        var password = "testpass";
        
        // QueryService throws exception that propagates
        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<User>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new InvalidOperationException("Critical error"));

        // Act
        var result = await _authService.ValidateUserAsync(username, password);

        // Assert
        result.Should().BeNull();
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
    public async Task ValidateUserAsync_LogsInformation_WhenValidating()
    {
        // Arrange
        var username = "testuser";
        var password = "testpass";
        var expectedUser = TestHelpers.CreateTestUser(username: username, password: password);
        
        _mockQueryService.Setup(x => x.QueryFirstOrDefaultAsync<User>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ReturnsAsync(expectedUser);

        // Act
        await _authService.ValidateUserAsync(username, password);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Information,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.AtLeastOnce);
    }

    [Fact]
    public async Task ValidateUserAsync_LogsDebug_WhenQueryFails()
    {
        // Arrange
        var username = "testuser";
        var password = "testpass";
        var expectedUser = TestHelpers.CreateTestUser(username: username, password: password);
        
        // First query fails, second succeeds
        _mockQueryService.SetupSequence(x => x.QueryFirstOrDefaultAsync<User>(
                It.IsAny<string>(),
                It.IsAny<object>()))
            .ThrowsAsync(new Exception("Table not found"))
            .ReturnsAsync(expectedUser);

        // Act
        await _authService.ValidateUserAsync(username, password);

        // Assert
        _mockLogger.Verify(
            x => x.Log(
                LogLevel.Debug,
                It.IsAny<EventId>(),
                It.Is<It.IsAnyType>((v, t) => true),
                It.IsAny<Exception>(),
                It.Is<Func<It.IsAnyType, Exception?, string>>((v, t) => true)),
            Times.Once);
    }
}

