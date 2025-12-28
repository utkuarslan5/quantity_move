using FluentAssertions;
using Microsoft.Extensions.Configuration;
using quantity_move_api.Services;

namespace quantity_move_api.Tests.Services;

public class ConfigurationServiceTests
{
    [Fact]
    public void GetDefaultWarehouse_WithConfiguration_ReturnsConfiguredValue()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Defaults:DefaultWarehouse", "TEST_WH" }
        });
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetDefaultWarehouse();

        // Assert
        result.Should().Be("TEST_WH");
    }

    [Fact]
    public void GetDefaultWarehouse_WithoutConfiguration_ReturnsDefault()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetDefaultWarehouse();

        // Assert
        result.Should().Be("MAIN");
    }

    [Fact]
    public void GetDefaultSite_WithConfiguration_ReturnsConfiguredValue()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Defaults:DefaultSite", "TEST_SITE" }
        });
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetDefaultSite();

        // Assert
        result.Should().Be("TEST_SITE");
    }

    [Fact]
    public void GetDefaultSite_WithoutConfiguration_ReturnsDefault()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetDefaultSite();

        // Assert
        result.Should().Be("Default");
    }

    [Fact]
    public void GetQualityControlEnabled_WithTrueConfiguration_ReturnsTrue()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Defaults:QualityControlEnabled", "true" }
        });
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetQualityControlEnabled();

        // Assert
        result.Should().BeTrue();
    }

    [Fact]
    public void GetQualityControlEnabled_WithFalseConfiguration_ReturnsFalse()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Defaults:QualityControlEnabled", "false" }
        });
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetQualityControlEnabled();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetQualityControlEnabled_WithoutConfiguration_ReturnsFalse()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetQualityControlEnabled();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetQualityControlEnabled_WithInvalidConfiguration_ReturnsFalse()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Defaults:QualityControlEnabled", "invalid" }
        });
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetQualityControlEnabled();

        // Assert
        result.Should().BeFalse();
    }

    [Fact]
    public void GetConnectionString_WithConfiguration_ReturnsConnectionString()
    {
        // Arrange
        var connectionString = "Server=test;Database=test;Trusted_Connection=true;";
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "ConnectionStrings:DefaultConnection", connectionString }
        });
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetConnectionString();

        // Assert
        result.Should().Be(connectionString);
    }

    [Fact]
    public void GetConnectionString_WithoutConfiguration_ThrowsException()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        Action act = () => service.GetConnectionString();

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Database connection string 'DefaultConnection' not found.");
    }

    [Fact]
    public void GetStoredProcedureName_WithConfiguration_ReturnsProcedureName()
    {
        // Arrange
        var procedureName = "TR_Miktar_Ilerlet";
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "StoredProcedures:MoveQuantity", procedureName }
        });
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetStoredProcedureName("MoveQuantity");

        // Assert
        result.Should().Be(procedureName);
    }

    [Fact]
    public void GetStoredProcedureName_WithoutConfiguration_ThrowsException()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        Action act = () => service.GetStoredProcedureName("MoveQuantity");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Stored procedure name for key 'MoveQuantity' not found in configuration.");
    }

    [Fact]
    public void GetTableName_WithConfiguration_ReturnsTableName()
    {
        // Arrange
        var tableName = "lot_location";
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>
        {
            { "Database:Tables:LotLocation", tableName }
        });
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        var result = service.GetTableName("LotLocation");

        // Assert
        result.Should().Be(tableName);
    }

    [Fact]
    public void GetTableName_WithoutConfiguration_ThrowsException()
    {
        // Arrange
        var configBuilder = new ConfigurationBuilder();
        configBuilder.AddInMemoryCollection(new Dictionary<string, string?>());
        var configuration = configBuilder.Build();
        var service = new ConfigurationService(configuration);

        // Act
        Action act = () => service.GetTableName("LotLocation");

        // Assert
        act.Should().Throw<InvalidOperationException>()
            .WithMessage("Table name for key 'LotLocation' not found in configuration.");
    }
}

