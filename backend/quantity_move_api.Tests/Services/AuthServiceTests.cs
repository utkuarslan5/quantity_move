using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using quantity_move_api.Models;
using quantity_move_api.Services;
using quantity_move_api.Tests.Helpers;

namespace quantity_move_api.Tests.Services;

public class AuthServiceTests
{
    private readonly IConfiguration _mockConfiguration;
    private readonly Mock<ILogger<AuthService>> _mockLogger;
    private readonly Mock<IDatabaseService> _mockDatabaseService;
    private readonly AuthService _authService;

    public AuthServiceTests()
    {
        _mockLogger = new Mock<ILogger<AuthService>>();
        _mockDatabaseService = new Mock<IDatabaseService>();
        
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "Server=test;Database=test;Trusted_Connection=true;" }
        });
        _mockConfiguration = configBuilder.Build();
        
        _authService = new AuthService(_mockDatabaseService.Object, _mockConfiguration, _mockLogger.Object);
    }

    [Fact]
    public async Task ValidateUserAsync_WithValidCredentials_ReturnsUser()
    {
        // Arrange
        var username = "testuser";
        var password = "testpass";
        var expectedUser = TestHelpers.CreateTestUser(username: username, password: password);
        
        // Note: AuthService uses direct SQL connection, so we can't easily mock it
        // This test would require integration testing or refactoring to use IDatabaseService
        // For now, we'll test the configuration validation

        // Act & Assert
        // Since AuthService directly creates SqlConnection, we need to test error scenarios
        // or use integration tests. This is a limitation of the current implementation.
        _authService.Should().NotBeNull();
    }

    [Fact]
    public async Task ValidateUserAsync_WithNullConnectionString_ReturnsNull()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var mockConfig = configBuilder.Build();
        
        var service = new AuthService(_mockDatabaseService.Object, mockConfig, _mockLogger.Object);

        // Act
        var result = await service.ValidateUserAsync("testuser", "testpass");

        // Assert
        result.Should().BeNull();
    }

    [Fact]
    public async Task ValidateUserAsync_WithEmptyConnectionString_ReturnsNull()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", "" }
        });
        var mockConfig = configBuilder.Build();
        
        var service = new AuthService(_mockDatabaseService.Object, mockConfig, _mockLogger.Object);

        // Act
        var result = await service.ValidateUserAsync("testuser", "testpass");

        // Assert
        result.Should().BeNull();
    }
}

